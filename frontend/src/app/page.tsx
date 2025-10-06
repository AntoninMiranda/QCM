// src/app/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { QcmParameterType, QcmParameterCategory } from "@/lib/parameter";

export default function Home() {
    const [qcmType, setQcmType] = useState("");
    const [qcmCategory, setQcmCategory] = useState("");
    const [qcmCount, setQcmCount] = useState("");
    const router = useRouter();

    const handleValidate = () => {
        let parameters = "?";

        if (qcmType) parameters += `type=${qcmType}&`;
        if (qcmCategory) parameters += `category=${qcmCategory}&`;
        if (qcmCount) parameters += `count=${qcmCount}`;
        console.log("home: " + parameters);
        router.push(`/qcm${parameters}`);
    };

    return (
        <div className="font-sans grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20">
            <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">
                <Card className="w-full max-w-sm">
                    <CardHeader>
                        <h2 className="text-lg font-bold">Choisissez le nombre de questions</h2>
                    </CardHeader>
                    <CardContent className="flex flex-col gap-4">
                        <Select value={qcmType} onValueChange={setQcmType}>
                            <SelectTrigger className="border rounded px-3 py-2">
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
                        <Select value={qcmCategory} onValueChange={setQcmCategory}>
                            <SelectTrigger className="border rounded px-3 py-2">
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
                        <Input
                            type="number"
                            min={1}
                            value={qcmCount}
                            onChange={e => setQcmCount(e.target.value)}
                            placeholder="Nombre de questions"
                        />
                    </CardContent>
                    <CardFooter className="flex justify-end">
                        <Button
                            variant="default"
                            disabled={!qcmType && !qcmCategory && !qcmCount}
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
