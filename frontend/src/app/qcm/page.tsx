// src/app/qcm/page.tsx
"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import { getData } from "@/api";
import { Card, CardHeader, CardContent, CardAction, CardFooter } from "@/components/ui/card";
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
        <Card className="w-full max-w-xl">
            <CardHeader>
                <h2 className="text-lg font-bold">{question.question}</h2>
            </CardHeader>
            <CardAction>
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

function QcmPageContent() {
    const [data, setData] = useState<any[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selected, setSelected] = useState<string | null>(null);
    const [validated, setValidated] = useState(false);
    const [result, setResult] = useState<{ score: number; total: number }>({ score: 0, total: 0 });
    const router = useRouter();
    const searchParams = useSearchParams();
    const nbQuestions = searchParams.get("nbQuestions") || "5";

    useEffect(() => {
        getData(nbQuestions)
            .then((res) => setData(res))
            .catch((err) => setError(err.message));
    }, [nbQuestions]);
    useEffect(() => {
        setResult({ score: 0, total: Number(nbQuestions) });
    }, [nbQuestions]);

    const nextQuestion = () => {
        if (currentIndex + 1 >= data.length) {
            router.push(`/qcm/result?score=${result.score}&total=${result.total}`);
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

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div>
                {error && <p>Erreur: {error}</p>}
                <QuestionModule
                    question={data[currentIndex]}
                    selected={selected}
                    setSelected={setSelected}
                    validated={validated}
                    onValidate={handleValidate}
                    onNext={handleNext}
                />
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
