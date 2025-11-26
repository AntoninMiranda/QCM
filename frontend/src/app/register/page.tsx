"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardHeader, CardContent, CardFooter, CardTitle, CardDescription } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { authApi } from "@/api";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";

export default function RegisterPage() {
    const [formData, setFormData] = useState({
        username: "",
        email: "",
        password: "",
        confirmPassword: "",
        firstName: "",
        lastName: "",
    });
    const [isLoading, setIsLoading] = useState(false);
    const router = useRouter();
    const { login } = useAuth();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (formData.password !== formData.confirmPassword) {
            toast.error("Les mots de passe ne correspondent pas");
            return;
        }

        if (formData.password.length < 6) {
            toast.error("Le mot de passe doit contenir au moins 6 caractères");
            return;
        }

        setIsLoading(true);

        try {
            const { confirmPassword, ...registerData } = formData;
            const response = await authApi.register(registerData);
            login(response.token, response.user);
            toast.success("Inscription réussie !");

            // Petit délai pour s'assurer que le localStorage est bien sauvegardé
            setTimeout(() => {
                router.push("/qcm");
            }, 100);
        } catch (error: any) {
            toast.error(error.message || "Erreur lors de l'inscription");
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            <Card className="w-full max-w-md">
                <CardHeader>
                    <CardTitle>Inscription</CardTitle>
                    <CardDescription>Créez votre compte</CardDescription>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <div className="space-y-2">
                            <Label htmlFor="username">Nom d&apos;utilisateur *</Label>
                            <Input
                                id="username"
                                name="username"
                                type="text"
                                placeholder="johndoe"
                                value={formData.username}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="email">Email *</Label>
                            <Input
                                id="email"
                                name="email"
                                type="email"
                                placeholder="votre@email.com"
                                value={formData.email}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <div className="grid grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="firstName">Prénom</Label>
                                <Input
                                    id="firstName"
                                    name="firstName"
                                    type="text"
                                    placeholder="John"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="lastName">Nom</Label>
                                <Input
                                    id="lastName"
                                    name="lastName"
                                    type="text"
                                    placeholder="Doe"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                />
                            </div>
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="password">Mot de passe *</Label>
                            <Input
                                id="password"
                                name="password"
                                type="password"
                                placeholder="••••••••"
                                value={formData.password}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="confirmPassword">Confirmer le mot de passe *</Label>
                            <Input
                                id="confirmPassword"
                                name="confirmPassword"
                                type="password"
                                placeholder="••••••••"
                                value={formData.confirmPassword}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <Button type="submit" className="w-full" disabled={isLoading}>
                            {isLoading ? "Inscription..." : "S'inscrire"}
                        </Button>
                    </form>
                </CardContent>
                <CardFooter className="flex flex-col space-y-2">
                    <div className="text-sm text-center">
                        Déjà un compte ?{" "}
                        <Link href="/login" className="text-primary hover:underline">
                            Se connecter
                        </Link>
                    </div>
                    <div className="text-sm text-center">
                        <Link href="/" className="text-primary hover:underline">
                            Retour à l&apos;accueil
                        </Link>
                    </div>
                </CardFooter>
            </Card>
        </div>
    );
}

