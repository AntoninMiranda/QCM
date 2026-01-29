// src/app/qcm/components/QcmModule.tsx
"use client";

import { Card, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { QuestionModule } from "./QuestionModule";
import { QcmQuestionGroup } from "../types";

interface QcmModuleProps {
    questionGroup: QcmQuestionGroup;
    selectedAnswers: (string | null)[];
    setSelectedAnswer: (index: number, key: string) => void;
    validated: boolean;
    onValidate: () => void;
    onNext: () => void;
    currentPage: number;
    totalPages: number;
}

export function QcmModule({
    questionGroup,
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
                {/* Afficher l'annexe si elle existe */}
                {questionGroup.annexes && questionGroup.annexes.trim() !== "" && (
                    <div className="mb-4 p-4 bg-blue-50 border border-blue-200 rounded-lg">
                        <h3 className="text-sm font-semibold text-blue-800 mb-2">Annexe :</h3>
                        <img 
                            src={`/Data/Annexes/${questionGroup.annexes}`} 
                            alt="Annexe" 
                            className="max-w-full h-auto rounded"
                            onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => {
                                // Fallback si l'image ne charge pas
                                e.currentTarget.style.display = 'none';
                                const nextElement = e.currentTarget.nextElementSibling as HTMLElement;
                                if (nextElement) {
                                    nextElement.textContent = `Fichier : ${questionGroup.annexes}`;
                                }
                            }}
                        />
                        <p className="text-sm text-blue-600 hidden">{questionGroup.annexes}</p>
                    </div>
                )}
                
                {questionGroup.questions.map((question, index) => (
                    <QuestionModule
                        key={`question-${index}`}
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

