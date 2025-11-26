"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardHeader, CardContent, CardFooter, CardTitle, CardDescription } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { authApi } from "@/api";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";

export default function ProfilePage() {
    const { user, token, isAuthenticated, logout, updateUser, isLoading: authLoading } = useAuth();
    const router = useRouter();
    const [isEditing, setIsEditing] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [formData, setFormData] = useState({
        username: "",
        email: "",
        firstName: "",
        lastName: "",
        currentPassword: "",
        newPassword: "",
        confirmNewPassword: "",
    });

    useEffect(() => {
        if (!authLoading && !isAuthenticated) {
            router.push("/login");
            return;
        }

        if (user) {
            setFormData({
                username: user.username || "",
                email: user.email || "",
                firstName: user.firstName || "",
                lastName: user.lastName || "",
                currentPassword: "",
                newPassword: "",
                confirmNewPassword: "",
            });
        }
    }, [user, isAuthenticated, authLoading, router]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (formData.newPassword && formData.newPassword !== formData.confirmNewPassword) {
            toast.error("Les nouveaux mots de passe ne correspondent pas");
            return;
        }

        if (formData.newPassword && formData.newPassword.length < 6) {
            toast.error("Le nouveau mot de passe doit contenir au moins 6 caractères");
            return;
        }

        if (!token) {
            toast.error("Session expirée. Veuillez vous reconnecter.");
            router.push("/login");
            return;
        }

        setIsLoading(true);

        try {
            const updateData: any = {
                username: formData.username,
                email: formData.email,
                firstName: formData.firstName || undefined,
                lastName: formData.lastName || undefined,
            };

            if (formData.newPassword) {
                updateData.currentPassword = formData.currentPassword;
                updateData.newPassword = formData.newPassword;
            }

            const response = await authApi.updateProfile(token, updateData);
            updateUser(response);
            toast.success("Profil mis à jour avec succès !");
            setIsEditing(false);
            setFormData({
                ...formData,
                currentPassword: "",
                newPassword: "",
                confirmNewPassword: "",
            });
        } catch (error: any) {
            toast.error(error.message || "Erreur lors de la mise à jour du profil");
        } finally {
            setIsLoading(false);
        }
    };

    const handleLogout = () => {
        logout();
        toast.success("Déconnexion réussie");
        router.push("/");
    };

    if (authLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <p>Chargement...</p>
            </div>
        );
    }

    if (!user) {
        return null;
    }

    return (
        <div className="min-h-screen">
            {/* Navigation */}
            <nav className="border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center h-16">
                        <Link href="/" className="flex-shrink-0">
                            <h1 className="text-xl font-bold">QCM App</h1>
                        </Link>
                        <div className="flex gap-4 items-center">
                            <Link href="/qcm">
                                <Button variant="outline">QCM</Button>
                            </Link>
                            <span className="text-sm">
                                {user?.username}
                            </span>
                        </div>
                    </div>
                </div>
            </nav>

            <div className="max-w-2xl mx-auto space-y-6 p-4 md:p-8">
                <div className="flex justify-between items-center">
                    <h1 className="text-3xl font-bold">Mon Profil</h1>
                </div>

                <Card>
                    <CardHeader>
                        <CardTitle>Informations personnelles</CardTitle>
                        <CardDescription>
                            {isEditing ? "Modifiez vos informations" : "Consultez vos informations"}
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div className="space-y-2">
                                <Label htmlFor="username">Nom d&apos;utilisateur</Label>
                                <Input
                                    id="username"
                                    name="username"
                                    type="text"
                                    value={formData.username}
                                    onChange={handleChange}
                                    disabled={!isEditing}
                                    required
                                />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="email">Email</Label>
                                <Input
                                    id="email"
                                    name="email"
                                    type="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    disabled={!isEditing}
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
                                        value={formData.firstName}
                                        onChange={handleChange}
                                        disabled={!isEditing}
                                    />
                                </div>
                                <div className="space-y-2">
                                    <Label htmlFor="lastName">Nom</Label>
                                    <Input
                                        id="lastName"
                                        name="lastName"
                                        type="text"
                                        value={formData.lastName}
                                        onChange={handleChange}
                                        disabled={!isEditing}
                                    />
                                </div>
                            </div>

                            {isEditing && (
                                <>
                                    <div className="border-t pt-4 mt-4">
                                        <h3 className="text-sm font-semibold mb-4">Changer le mot de passe (optionnel)</h3>
                                        <div className="space-y-4">
                                            <div className="space-y-2">
                                                <Label htmlFor="currentPassword">Mot de passe actuel</Label>
                                                <Input
                                                    id="currentPassword"
                                                    name="currentPassword"
                                                    type="password"
                                                    value={formData.currentPassword}
                                                    onChange={handleChange}
                                                    placeholder="••••••••"
                                                />
                                            </div>
                                            <div className="space-y-2">
                                                <Label htmlFor="newPassword">Nouveau mot de passe</Label>
                                                <Input
                                                    id="newPassword"
                                                    name="newPassword"
                                                    type="password"
                                                    value={formData.newPassword}
                                                    onChange={handleChange}
                                                    placeholder="••••••••"
                                                />
                                            </div>
                                            <div className="space-y-2">
                                                <Label htmlFor="confirmNewPassword">Confirmer le nouveau mot de passe</Label>
                                                <Input
                                                    id="confirmNewPassword"
                                                    name="confirmNewPassword"
                                                    type="password"
                                                    value={formData.confirmNewPassword}
                                                    onChange={handleChange}
                                                    placeholder="••••••••"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                </>
                            )}

                            {isEditing && (
                                <div className="flex gap-2 pt-4">
                                    <Button type="submit" disabled={isLoading}>
                                        {isLoading ? "Sauvegarde..." : "Sauvegarder"}
                                    </Button>
                                    <Button
                                        type="button"
                                        variant="outline"
                                        onClick={() => {
                                            setIsEditing(false);
                                            if (user) {
                                                setFormData({
                                                    username: user.username || "",
                                                    email: user.email || "",
                                                    firstName: user.firstName || "",
                                                    lastName: user.lastName || "",
                                                    currentPassword: "",
                                                    newPassword: "",
                                                    confirmNewPassword: "",
                                                });
                                            }
                                        }}
                                    >
                                        Annuler
                                    </Button>
                                </div>
                            )}
                        </form>
                    </CardContent>
                    {!isEditing && (
                        <CardFooter className="flex justify-between">
                            <Button onClick={() => setIsEditing(true)}>
                                Modifier le profil
                            </Button>
                            <Button variant="destructive" onClick={handleLogout}>
                                Se déconnecter
                            </Button>
                        </CardFooter>
                    )}
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle>Statistiques</CardTitle>
                        <CardDescription>Vos performances aux QCM</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p className="text-sm text-muted-foreground">
                            Les statistiques seront bientôt disponibles.
                        </p>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}

