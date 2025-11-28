// src/app/qcm/components/QcmParametersForm.tsx
"use client";

import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select";
import { QcmParameterType, QcmParameterCategory } from "@/lib/parameter";

interface QcmParametersFormProps {
    selectedType: string;
    selectedCategory: string;
    selectedCount: string;
    onTypeChange: (value: string) => void;
    onCategoryChange: (value: string) => void;
    onCountChange: (value: string) => void;
    onStart: () => void;
}

export function QcmParametersForm({
    selectedType,
    selectedCategory,
    selectedCount,
    onTypeChange,
    onCategoryChange,
    onCountChange,
    onStart
}: QcmParametersFormProps) {
    const isDisabled = !selectedType && !selectedCategory && !selectedCount;

    return (
        <Card className="w-full max-w-md">
            <CardHeader>
                <CardTitle>Paramètres du QCM</CardTitle>
            </CardHeader>
            <CardContent className="flex flex-col gap-4">
                <div className="space-y-2">
                    <label className="text-sm font-medium">Type de question</label>
                    <Select value={selectedType} onValueChange={onTypeChange}>
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
                    <Select value={selectedCategory} onValueChange={onCategoryChange}>
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
                        onChange={e => onCountChange(e.target.value)}
                        placeholder="Nombre de questions"
                    />
                </div>
            </CardContent>
            <CardFooter className="flex justify-end">
                <Button
                    variant="default"
                    disabled={isDisabled}
                    onClick={onStart}
                >
                    Commencer
                </Button>
            </CardFooter>
        </Card>
    );
}

