// src/app/qcm/components/QuestionModule.tsx
"use client";

import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { QcmQuestion } from "../types";

interface QuestionModuleProps {
    question: QcmQuestion;
    selected: string | null;
    setSelected: (index: number, key: string) => void;
    validated: boolean;
    index: number;
}

export function QuestionModule({
    question,
    selected,
    setSelected,
    validated,
    index
}: QuestionModuleProps) {
    if (!question) {
        return (
            <Card>
                <CardContent>Aucune question</CardContent>
            </Card>
        );
    }

    return (
        <Card className="w-full max-w-xl min-w-[350px] mb-4">
            <CardHeader>
                <h2 className="text-lg font-bold">{question.question}</h2>
            </CardHeader>
            <CardContent className="p-4 flex flex-col gap-2">
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
            </CardContent>
            <CardFooter>
                {question.id && <span className="text-sm text-gray-500">{question.id}</span>}
            </CardFooter>
        </Card>
    );
}

