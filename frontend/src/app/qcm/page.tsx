// src/app/qcm/page.tsx
"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import { getData } from "@/api";
import { Card, CardHeader, CardTitle, CardContent, CardAction, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";


function QuestionModule({ question, selected, setSelected, validated, onValidate, onNext }: {
    question: any;
    selected: string | null;
    setSelected: (key: string) => void;
    validated: boolean;
    onValidate: () => void;
    onNext: () => void;
}) {
    if (!question) return <Card><CardContent>Aucune question</CardContent></Card>;
    return (
        <Card className="w-full max-w-xl min-w-[350px] min-h-[400px] flex flex-col justify-between">
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
                            onClick={() => !validated && setSelected(key)}
                            className={validated ? "pointer-events-none" : ""}
                        >
                            {key} : {String(value)}
                        </Button>
                    );
                })}
            </CardAction>
            <CardFooter className="flex justify-end">
                {validated ? (
                    <Button variant="default" onClick={onNext}>
                        Question suivante
                    </Button>
                ) : (
                    <Button
                        variant="default"
                        onClick={onValidate}
                        disabled={!selected}
                    >
                        Valider
                    </Button>
                )}
            </CardFooter>
        </Card>
    );
}

function ResultModule( { result, handleRestart }: {
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
            <CardFooter className="flex justify-end">
                <Button onClick={() => router.push("/")}>Accueil</Button>
                <Button onClick={handleRestart}>Recommencer</Button>
            </CardFooter>
        </Card>
    );
}

function QcmPageContent() {
    const [data, setData] = useState<any[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selected, setSelected] = useState<string | null>(null);
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
    }, [qcmCount]);
    useEffect(() => {
        setResult({ score: 0, total: Number(data.length) });
    }, [data]);

    const nextQuestion = () => {
        if (currentIndex + 1 >= data.length) {
            setShowResult(true);
        } else {
            setCurrentIndex((prev) => prev + 1);
        }
    };
    const handleValidate = () => {
        setValidated(true);
        if (selected === data[currentIndex].answer) {
            setResult(prev => ({ score: prev.score + 1, total: prev.total }));
        }
    };
    const handleNext = () => {
        setValidated(false);
        setSelected(null);
        nextQuestion();
    };
    const handleRestart = () => {
        setShowResult(false);
        setCurrentIndex(0);
        setSelected(null);
        setValidated(false);
        setResult({ score: 0, total: Number(qcmCount) });
    };

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div>
                {error && <p>Erreur: {error}</p>}
                {!showResult ? (
                    <QuestionModule
                        question={data[currentIndex]}
                        selected={selected}
                        setSelected={setSelected}
                        validated={validated}
                        onValidate={handleValidate}
                        onNext={handleNext}
                    />
                ) : (
                    <ResultModule
                        result={result}
                        handleRestart={handleRestart}
                    />
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
