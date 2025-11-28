// src/app/qcm/components/QcmNavigation.tsx
"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";

interface QcmNavigationProps {
    username?: string;
}

export function QcmNavigation({ username }: QcmNavigationProps) {
    return (
        <nav className="border-b">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between items-center h-16">
                    <Link href="/" className="flex-shrink-0">
                        <h1 className="text-xl font-bold">QCM App</h1>
                    </Link>
                    <div className="flex gap-4 items-center">
                        <span className="text-sm">
                            Bonjour, {username}
                        </span>
                        <Link href="/profile">
                            <Button variant="outline">Mon profil</Button>
                        </Link>
                    </div>
                </div>
            </div>
        </nav>
    );
}

