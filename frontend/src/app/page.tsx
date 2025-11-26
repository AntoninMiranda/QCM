// src/app/page.tsx
"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardContent, CardTitle, CardDescription } from "@/components/ui/card";
import { useAuth } from "@/hooks/useAuth";

export default function Home() {
    const { isAuthenticated, user } = useAuth();

    return (
        <div className="font-sans min-h-screen">
            {/* Navigation */}
            <nav className="border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center h-16">
                        <div className="flex-shrink-0">
                            <h1 className="text-xl font-bold">QCM App</h1>
                        </div>
                        <div className="flex gap-4">
                            {isAuthenticated ? (
                                <>
                                    <span className="text-sm flex items-center">
                                        Bonjour, {user?.username}
                                    </span>
                                    <Link href="/profile">
                                        <Button variant="outline">Mon profil</Button>
                                    </Link>
                                </>
                            ) : (
                                <>
                                    <Link href="/login">
                                        <Button variant="outline">Connexion</Button>
                                    </Link>
                                    <Link href="/register">
                                        <Button>Inscription</Button>
                                    </Link>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </nav>

            {/* Main Content */}
            <div className="flex items-center justify-center min-h-[calc(100vh-4rem)] p-8">
                <div className="max-w-2xl w-full space-y-8">
                    <div className="text-center space-y-4">
                        <h1 className="text-4xl font-bold tracking-tight">
                            Bienvenue sur QCM App
                        </h1>
                        <p className="text-xl text-muted-foreground">
                            Testez vos connaissances avec nos questionnaires à choix multiples
                        </p>
                    </div>

                    {isAuthenticated ? (
                        <Card>
                            <CardHeader>
                                <CardTitle>Prêt à commencer ?</CardTitle>
                                <CardDescription>
                                    Accédez aux QCM pour tester vos connaissances
                                </CardDescription>
                            </CardHeader>
                            <CardContent className="flex justify-center">
                                <Link href="/qcm">
                                    <Button size="lg">Commencer un QCM</Button>
                                </Link>
                            </CardContent>
                        </Card>
                    ) : (
                        <Card>
                            <CardHeader>
                                <CardTitle>Connectez-vous pour commencer</CardTitle>
                                <CardDescription>
                                    Créez un compte ou connectez-vous pour accéder aux QCM
                                </CardDescription>
                            </CardHeader>
                            <CardContent className="flex flex-col sm:flex-row gap-4 justify-center">
                                <Link href="/register">
                                    <Button size="lg" className="w-full sm:w-auto">
                                        S&apos;inscrire
                                    </Button>
                                </Link>
                                <Link href="/login">
                                    <Button size="lg" variant="outline" className="w-full sm:w-auto">
                                        Se connecter
                                    </Button>
                                </Link>
                            </CardContent>
                        </Card>
                    )}
                </div>
            </div>
        </div>
    );
}
