// src/app/qcm/page.tsx
"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import Link from "next/link";
import { getData } from "@/api";
import { Card, CardHeader, CardTitle, CardContent, CardAction, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select";
import { QcmParameterType, QcmParameterCategory } from "@/lib/parameter";
import { useAuth } from "@/hooks/useAuth";

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
                            className={`w-full text-left flex justify-start items-center break-words whitespace-normal h-auto py-2 ${validated ? "pointer-events-none" : ""}`}
                        >
                            <span className="mr-2">{key} :</span>
                            <span className="flex-grow">{String(value)}</span>
                        </Button>
                    );
                })}
            </CardAction>
            <CardFooter>
                {question.id && <span className="text-sm text-gray-500">{question.id}</span>}
            </CardFooter>
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
            <CardFooter className="flex justify-end gap-2">
                <Button onClick={() => router.push("/qcm")} variant="outline">Nouveau QCM</Button>
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
    const router = useRouter();
    const { isAuthenticated, user, isLoading: authLoading } = useAuth();
    const qcmType = searchParams.get("type");
    const qcmCategory = searchParams.get("category") || null;
    const qcmCount = searchParams.get("count") || null;

    // État pour le formulaire de sélection
    const [selectedType, setSelectedType] = useState("");
    const [selectedCategory, setSelectedCategory] = useState("");
    const [selectedCount, setSelectedCount] = useState("");
    const [hasParameters, setHasParameters] = useState(false);

    useEffect(() => {
        // Vérifier si des paramètres sont fournis
        if (qcmType || qcmCategory || qcmCount) {
            setHasParameters(true);
            let parameter = "?";
            if (qcmType) parameter += `type=${qcmType}&`;
            if (qcmCategory) parameter += `category=${qcmCategory}&`;
            if (qcmCount) parameter += `count=${qcmCount}`;

            // Récupérer le token depuis localStorage
            const token = localStorage.getItem('token');

            getData(parameter, token || undefined)
                .then((res) => setData(res))
                .catch((err) => setError(err.message));
        } else {
            setHasParameters(false);
        }
    }, [qcmType, qcmCategory, qcmCount]);

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

    const handleStartQcm = () => {
        let parameters = "?";
        if (selectedType) parameters += `type=${selectedType}&`;
        if (selectedCategory) parameters += `category=${selectedCategory}&`;
        if (selectedCount) parameters += `count=${selectedCount}`;
        router.push(`/qcm${parameters}`);
    };

    // Vérifier l'authentification
    useEffect(() => {
        if (!authLoading && !isAuthenticated) {
            router.push("/login");
        }
    }, [isAuthenticated, authLoading, router]);

    if (authLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <p>Chargement...</p>
            </div>
        );
    }

    if (!isAuthenticated) {
        return null;
    }

    return (
        <div className="font-sans min-h-screen">
            {/* Navigation */}
            <nav className="border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center h-16">
                        <Link href="/" className="flex-shrink-0">
                            <h1 className="text-xl font-bold">QCM App</h1>
                        </Link>
                        <div className="flex gap-4 items-center">
                            <span className="text-sm">
                                Bonjour, {user?.username}
                            </span>
                            <Link href="/profile">
                                <Button variant="outline">Mon profil</Button>
                            </Link>
                        </div>
                    </div>
                </div>
            </nav>

            {/* Main Content */}
            <div className="flex items-center justify-center min-h-[calc(100vh-4rem)] p-4">
            <div>
                {!hasParameters ? (
                    // Formulaire de sélection des paramètres
                    <Card className="w-full max-w-md">
                        <CardHeader>
                            <CardTitle>Paramètres du QCM</CardTitle>
                        </CardHeader>
                        <CardContent className="flex flex-col gap-4">
                            <div className="space-y-2">
                                <label className="text-sm font-medium">Type de question</label>
                                <Select value={selectedType} onValueChange={setSelectedType}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Type de question..." />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {QcmParameterType.map(opt => (
                                            <SelectItem key={opt.value} value={opt.value}>
                                                {opt.label}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                            <div className="space-y-2">
                                <label className="text-sm font-medium">Catégorie</label>
                                <Select value={selectedCategory} onValueChange={setSelectedCategory}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Catégorie de question..." />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {QcmParameterCategory.map(opt => (
                                            <SelectItem key={opt.value} value={opt.value}>
                                                {opt.label}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                            <div className="space-y-2">
                                <label className="text-sm font-medium">Nombre de questions</label>
                                <Input
                                    type="number"
                                    min={1}
                                    value={selectedCount}
                                    onChange={e => setSelectedCount(e.target.value)}
                                    placeholder="Nombre de questions"
                                />
                            </div>
                        </CardContent>
                        <CardFooter className="flex justify-end">
                            <Button
                                variant="default"
                                disabled={!selectedType && !selectedCategory && !selectedCount}
                                onClick={handleStartQcm}
                            >
                                Commencer
                            </Button>
                        </CardFooter>
                    </Card>
                ) : (
                    // Affichage du QCM ou du résultat
                    <>
                        {error && <p className="text-red-500 mb-4">Erreur: {error}</p>}
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
                            <Card><CardContent className="pt-6">Chargement des questions...</CardContent></Card>
                        )}
                    </>
                )}
            </div>
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
