// src/app/qcm/hooks/useQcmLogic.ts
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { getData, scoreApi } from "@/api";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";

interface UseQcmLogicProps {
    qcmType: string | null;
    qcmCategory: string | null;
    qcmCount: string | null;
}

export function useQcmLogic({ qcmType, qcmCategory, qcmCount }: UseQcmLogicProps) {
    const [data, setData] = useState<any[][]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selectedAnswers, setSelectedAnswers] = useState<(string | null)[]>([]);
    const [validated, setValidated] = useState(false);
    const [result, setResult] = useState<{ score: number; total: number }>({ score: 0, total: 0 });
    const [showResult, setShowResult] = useState(false);
    const [startTime, setStartTime] = useState<number>(Date.now());
    const [questionDetails, setQuestionDetails] = useState<any[]>([]);
    const [isSavingScore, setIsSavingScore] = useState(false);
    const [isFirstAttempt, setIsFirstAttempt] = useState(true);
    const [hasParameters, setHasParameters] = useState(false);

    const { isAuthenticated, token } = useAuth();
    const router = useRouter();

    // Charger les données du QCM
    useEffect(() => {
        if (qcmType || qcmCategory || qcmCount) {
            setHasParameters(true);
            setIsFirstAttempt(true);

            let parameter = "?";
            if (qcmType) parameter += `type=${qcmType}&`;
            if (qcmCategory) parameter += `category=${qcmCategory}&`;
            if (qcmCount) parameter += `count=${qcmCount}`;

            const token = localStorage.getItem('token');

            getData(parameter, token || undefined)
                .then((res) => setData(res))
                .catch((err) => setError(err.message));
        } else {
            setHasParameters(false);
        }
    }, [qcmType, qcmCategory, qcmCount]);

    // Initialiser les réponses pour la page actuelle
    useEffect(() => {
        if (data.length > 0 && data[currentIndex]) {
            setResult({ score: result.score, total: data.flat().length });
            setSelectedAnswers(new Array(data[currentIndex].length).fill(null));
            setValidated(false);
        }
    }, [data, currentIndex]);


    const handleSetSelected = (index: number, value: string) => {
        setSelectedAnswers(prev => {
            const newAnswers = [...prev];
            newAnswers[index] = value;
            return newAnswers;
        });
    };

    const handleValidate = () => {
        setValidated(true);
        let correctAnswers = 0;

        const pageDetails = data[currentIndex].map((question, index) => {
            const isCorrect = selectedAnswers[index] === question.answer;
            if (isCorrect) correctAnswers++;

            return {
                questionId: question.id,
                question: question.question,
                userAnswer: selectedAnswers[index],
                correctAnswer: question.answer,
                isCorrect: isCorrect
            };
        });

        const newQuestionDetails = [...questionDetails, ...pageDetails];
        const newScore = result.score + correctAnswers;

        setQuestionDetails(newQuestionDetails);
        setResult(prev => ({
            score: newScore,
            total: prev.total
        }));
    };


    const handleNext = () => {
        setValidated(false);
        nextQuestion();
    };

    const nextQuestion = () => {
        if (currentIndex + 1 >= data.length) {
            setShowResult(true);
        } else {
            setCurrentIndex((prev) => prev + 1);
        }
    };

    const saveScoreToBackend = async (finalScore: number, finalDetails: any[]) => {
        if (!token || !isAuthenticated || isSavingScore || !isFirstAttempt) {
            if (!isFirstAttempt) {
                console.log("Score non enregistré : ce n'est pas le premier essai");
            }
            return;
        }

        setIsSavingScore(true);
        try {
            const timeSpent = Math.floor((Date.now() - startTime) / 1000);

            const scoreData = {
                qcmType: qcmType || "mixed",
                category: qcmCategory || undefined,
                totalQuestions: result.total,
                correctAnswers: finalScore,
                incorrectAnswers: result.total - finalScore,
                timeSpentSeconds: timeSpent,
                details: JSON.stringify(finalDetails)
            };

            await scoreApi.saveScore(token, scoreData);
            toast.success("Score enregistré avec succès !");
            setIsFirstAttempt(false);
        } catch (error: any) {
            console.error("Erreur lors de l'enregistrement du score:", error);
            toast.error(error.message || "Erreur lors de l'enregistrement du score");
        } finally {
            setIsSavingScore(false);
        }
    };

    const handleRestart = () => {
        setShowResult(false);
        setCurrentIndex(0);
        setSelectedAnswers([]);
        setValidated(false);
        setResult({ score: 0, total: data.flat().length });
        setStartTime(Date.now());
        setQuestionDetails([]);
        setIsSavingScore(false);
    };

    const handleStartQcm = (selectedType: string, selectedCategory: string, selectedCount: string) => {
        let parameters = "?";
        if (selectedType) parameters += `type=${selectedType}&`;
        if (selectedCategory) parameters += `category=${selectedCategory}&`;
        if (selectedCount) parameters += `count=${selectedCount}`;
        router.push(`/qcm${parameters}`);
    };

    const handleNewQcm = () => {
        // Réinitialiser TOUS les états
        setShowResult(false);
        setCurrentIndex(0);
        setSelectedAnswers([]);
        setValidated(false);
        setResult({ score: 0, total: 0 });
        setStartTime(Date.now());
        setQuestionDetails([]);
        setIsSavingScore(false);
        setIsFirstAttempt(true);
        setData([]);
        setError(null);
        setHasParameters(false);

        // Naviguer vers la page de sélection
        router.push("/qcm");
    };

    const handleSaveScore = () => {
        if (isFirstAttempt && result.score >= 0) {
            saveScoreToBackend(result.score, questionDetails);
        }
    };

    return {
        // États
        data,
        error,
        currentIndex,
        selectedAnswers,
        validated,
        result,
        showResult,
        hasParameters,
        isFirstAttempt,
        totalPages: data.length,

        // Fonctions
        handleSetSelected,
        handleValidate,
        handleNext,
        handleRestart,
        handleStartQcm,
        handleNewQcm,
        handleSaveScore
    };
}

