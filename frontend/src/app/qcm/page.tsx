// src/app/qcm/page.tsx
"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import { getData } from "@/api";
import { Card, CardHeader, CardTitle, CardContent, CardAction, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

function QuestionModule({ question, selected, setSelected, validated, index }: {
    question: any;
    selected: string | null;
    setSelected: (index: number, key: string) => void;
    validated: boolean;
    index: number;
}) {
    if (!question) return <Card><CardContent>Aucune question</CardContent></Card>;
    return (
        <Card className="w-full max-w-xl min-w-[350px] mb-4">
            <CardHeader>
                <h2 className="text-lg font-bold">{question.question}</h2>
            </CardHeader>
            <CardAction className="p-4 flex flex-col gap-2">
                {Object.entries(question.choices).map(([key, value]) => {
                    let variant: "link" | "outline" | "success" | "destructive" | "default" | "secondary" | "ghost" | undefined = "outline";
                    if (validated) {
                        if (key === question.answer) variant = "success";
                        else if (key === selected) variant = "destructive";
                    } else if (key === selected) {
                        variant = "default";
                    }
                    return (
                        <Button
                            key={key}
                            variant={variant}
                            onClick={() => !validated && setSelected(index, key)}
                            className={validated ? "pointer-events-none" : ""}
                        >
                            {key} : {String(value)}
                        </Button>
                    );
                })}
            </CardAction>
        </Card>
    );
}

function QcmModule({ questions, selectedAnswers, setSelectedAnswer, validated, onValidate, onNext }: {
    questions: any[];
    selectedAnswers: (string | null)[];
    setSelectedAnswer: (index: number, key: string) => void;
    validated: boolean;
    onValidate: () => void;
    onNext: () => void;
}) {
    const allQuestionsAnswered = selectedAnswers.every(answer => answer !== null);

    return (
        <Card className="w-full max-w-xl min-w-[350px] flex flex-col justify-between">
            <CardContent className="pt-6">
                {questions.map((question, index) => (
                    <QuestionModule
                        key={index}
                        question={question}
                        selected={selectedAnswers[index]}
                        setSelected={setSelectedAnswer}
                        validated={validated}
                        index={index}
                    />
                ))}
            </CardContent>
            <CardFooter className="flex justify-end">
                {validated ? (
                    <Button variant="default" onClick={onNext}>
                        Page suivante
                    </Button>
                ) : (
                    <Button
                        variant="default"
                        onClick={onValidate}
                        // disabled={!allQuestionsAnswered}
                    >
                        Valider
                    </Button>
                )}
            </CardFooter>
        </Card>
    );
}

function ResultModule({ result, handleRestart }: {
    result: { score: number; total: number };
    handleRestart: () => void
}) {
    // Le reste du code reste inchangé
    const router = useRouter();

    return (
        <Card className="w-[350px]">
            <CardHeader>
                <CardTitle>Résultat du QCM</CardTitle>
            </CardHeader>
            <CardContent>
                <p className="text-lg mb-2">
                    Vous avez obtenu {result.score} / {result.total} bonnes réponses.
                </p>
                {result.score === result.total ? (
                    <p className="text-green-600 font-semibold">Félicitations, score parfait !</p>
                ) : (
                    <p className="text-blue-600">Essayez encore pour améliorer votre score.</p>
                )}
            </CardContent>
            <CardFooter className="flex justify-end">
                <Button onClick={() => router.push("/")}>Accueil</Button>
                <Button onClick={handleRestart}>Recommencer</Button>
            </CardFooter>
        </Card>
    );
}

function QcmPageContent() {
    const [data, setData] = useState<any[][]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selectedAnswers, setSelectedAnswers] = useState<(string | null)[]>([]);
    const [validated, setValidated] = useState(false);
    const [result, setResult] = useState<{ score: number; total: number }>({ score: 0, total: 0 });
    const [showResult, setShowResult] = useState(false);
    const searchParams = useSearchParams();
    const qcmType = searchParams.get("type");
    const qcmCategory = searchParams.get("category") || null;
    const qcmCount = searchParams.get("count") || null;

    useEffect(() => {
        let parameter = "?";
        parameter += `type=${qcmType}&`;
        if (qcmCategory) parameter += `category=${qcmCategory}&`;
        if (qcmCount) parameter += `count=${qcmCount}`;
        getData(parameter)
            .then((res) => setData(res))
            .catch((err) => setError(err.message));
    }, [qcmType, qcmCategory, qcmCount]);

    useEffect(() => {
        if (data.length > 0 && data[currentIndex]) {
            setResult({ score: 0, total: data.flat().length });
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

    const nextQuestion = () => {
        if (currentIndex + 1 >= data.length) {
            setShowResult(true);
        } else {
            setCurrentIndex((prev) => prev + 1);
        }
    };

    const handleValidate = () => {
        setValidated(true);
        let correctAnswers = 0;

        data[currentIndex].forEach((question, index) => {
            if (selectedAnswers[index] === question.answer) {
                correctAnswers++;
            }
        });

        setResult(prev => ({
            score: prev.score + correctAnswers,
            total: prev.total
        }));
    };

    const handleNext = () => {
        setValidated(false);
        nextQuestion();
    };

    const handleRestart = () => {
        setShowResult(false);
        setCurrentIndex(0);
        setSelectedAnswers([]);
        setValidated(false);
        setResult({ score: 0, total: data.flat().length });
    };

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div>
                {error && <p>Erreur: {error}</p>}
                {!showResult && data.length > 0 && data[currentIndex] ? (
                    <QcmModule
                        questions={data[currentIndex]}
                        selectedAnswers={selectedAnswers}
                        setSelectedAnswer={handleSetSelected}
                        validated={validated}
                        onValidate={handleValidate}
                        onNext={handleNext}
                    />
                ) : showResult ? (
                    <ResultModule
                        result={result}
                        handleRestart={handleRestart}
                    />
                ) : (
                    <Card><CardContent>Chargement des questions...</CardContent></Card>
                )}
            </div>
        </div>
    );
}

export default function QcmPage() {
    return (
        <Suspense fallback={<div>Chargement...</div>}>
            <QcmPageContent />
        </Suspense>
    );
}
