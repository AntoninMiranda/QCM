// src/app/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

export default function Home() {
    const [nbQuestions, setNbQuestions] = useState("");
    const router = useRouter();

    const handleValidate = () => {
        if (nbQuestions) {
            router.push(`/qcm?nbQuestions=${nbQuestions}`);
        }
    };

    return (
        <div className="font-sans grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20">
            <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">
                <Card className="w-full max-w-sm">
                    <CardHeader>
                        <h2 className="text-lg font-bold">Choisissez le nombre de questions</h2>
                    </CardHeader>
                    <CardContent className="flex flex-col gap-4">
                        <input
                            type="number"
                            min={1}
                            value={nbQuestions}
                            onChange={e => setNbQuestions(e.target.value)}
                            className="border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Nombre de questions"
                        />
                    </CardContent>
                    <CardFooter className="flex justify-end">
                        <Button
                            variant="default"
                            disabled={!nbQuestions}
                            onClick={handleValidate}
                        >
                            Valider
                        </Button>
                    </CardFooter>
                </Card>
            </main>
        </div>
    );
}
