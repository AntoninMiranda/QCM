// src/app/qcm/page.tsx
"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { useAuth } from "@/hooks/useAuth";
import { useQcmLogic } from "./hooks";
import {
    QcmModule,
    ResultModule,
    QcmParametersForm,
    QcmNavigation
} from "./components";

function QcmPageContent() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const { isAuthenticated, user, isLoading: authLoading } = useAuth();

    const qcmType = searchParams.get("type");
    const qcmCategory = searchParams.get("category") || null;
    const qcmCount = searchParams.get("count") || null;

    // États pour le formulaire de sélection
    const [selectedType, setSelectedType] = useState("");
    const [selectedCategory, setSelectedCategory] = useState("");
    const [selectedCount, setSelectedCount] = useState("");

    // Utiliser le hook personnalisé pour gérer toute la logique du QCM
    const {
        data,
        error,
        currentIndex,
        selectedAnswers,
        validated,
        result,
        showResult,
        hasParameters,
        isFirstAttempt,
        handleSetSelected,
        handleValidate,
        handleNext,
        handleRestart,
        handleStartQcm: startQcm,
        handleSaveScore
    } = useQcmLogic({ qcmType, qcmCategory, qcmCount });

    const handleStartQcm = () => {
        startQcm(selectedType, selectedCategory, selectedCount);
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
            <QcmNavigation username={user?.username} />

            <div className="flex items-center justify-center min-h-[calc(100vh-4rem)] p-4">
                <div>
                    {!hasParameters ? (
                        <QcmParametersForm
                            selectedType={selectedType}
                            selectedCategory={selectedCategory}
                            selectedCount={selectedCount}
                            onTypeChange={setSelectedType}
                            onCategoryChange={setSelectedCategory}
                            onCountChange={setSelectedCount}
                            onStart={handleStartQcm}
                        />
                    ) : (
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
                                    onSaveScore={isFirstAttempt ? handleSaveScore : undefined}
                                />
                            ) : (
                                <Card>
                                    <CardContent className="pt-6">
                                        Chargement des questions...
                                    </CardContent>
                                </Card>
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
