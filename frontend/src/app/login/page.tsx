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

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const router = useRouter();
    const { login } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);

        try {
            const response = await authApi.login({ email, password });
            login(response.token, response.user);
            toast.success("Connexion réussie !");

            // Petit délai pour s'assurer que le localStorage est bien sauvegardé
            setTimeout(() => {
                router.push("/qcm");
            }, 100);
        } catch (error: any) {
            toast.error(error.message || "Erreur lors de la connexion");
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            <Card className="w-full max-w-md">
                <CardHeader>
                    <CardTitle>Connexion</CardTitle>
                    <CardDescription>Connectez-vous à votre compte</CardDescription>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <div className="space-y-2">
                            <Label htmlFor="email">Email</Label>
                            <Input
                                id="email"
                                type="email"
                                placeholder="votre@email.com"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="password">Mot de passe</Label>
                            <Input
                                id="password"
                                type="password"
                                placeholder="••••••••"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>
                        <Button type="submit" className="w-full" disabled={isLoading}>
                            {isLoading ? "Connexion..." : "Se connecter"}
                        </Button>
                    </form>
                </CardContent>
                <CardFooter className="flex flex-col space-y-2">
                    <div className="text-sm text-center">
                        Pas encore de compte ?{" "}
                        <Link href="/register" className="text-primary hover:underline">
                            S&apos;inscrire
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

