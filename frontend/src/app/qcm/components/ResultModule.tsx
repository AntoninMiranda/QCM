// src/app/qcm/components/ResultModule.tsx
"use client";

import { useEffect, useRef } from "react";
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

interface ResultModuleProps {
    result: {
        score: number;
        total: number
    };
    handleRestart: () => void;
    handleNewQcm: () => void;
    onSaveScore?: () => void | Promise<void>;
}

export function ResultModule({ result, handleRestart, handleNewQcm, onSaveScore }: ResultModuleProps) {
    const hasSaved = useRef(false);

    // Sauvegarder le score au montage du composant (première fois uniquement)
    useEffect(() => {
        if (!hasSaved.current && onSaveScore) {
            hasSaved.current = true;
            onSaveScore();
        }
    }, [onSaveScore]);

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
                <Button onClick={handleNewQcm} variant="outline">
                    Nouveau QCM
                </Button>
                <Button onClick={handleRestart}>
                    Recommencer
                </Button>
            </CardFooter>
        </Card>
    );
}

