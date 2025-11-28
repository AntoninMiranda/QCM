// src/app/qcm/components/QcmModule.tsx
"use client";

import { Card, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { QuestionModule } from "./QuestionModule";

interface QcmModuleProps {
    questions: any[];
    selectedAnswers: (string | null)[];
    setSelectedAnswer: (index: number, key: string) => void;
    validated: boolean;
    onValidate: () => void;
    onNext: () => void;
    currentPage: number;
    totalPages: number;
}

export function QcmModule({
    questions,
    selectedAnswers,
    setSelectedAnswer,
    validated,
    onValidate,
    onNext,
    currentPage,
    totalPages
}: QcmModuleProps) {
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
            <CardFooter className="flex justify-between items-center">
                <span className="text-sm text-muted-foreground font-medium">
                    {currentPage + 1}/{totalPages}
                </span>
                {validated ? (
                    <Button variant="default" onClick={onNext}>
                        Page suivante
                    </Button>
                ) : (
                    <Button variant="default" onClick={onValidate}>
                        Valider
                    </Button>
                )}
            </CardFooter>
        </Card>
    );
}

