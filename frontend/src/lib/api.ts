// Déclaration globale pour window.ENV
declare global {
    interface Window {
        ENV?: {
            API_URL: string;
        };
    }
}

// Fonction pour obtenir l'URL de l'API
export function getApiUrl(): string {
    // En runtime, utiliser window.ENV
    if (typeof window !== 'undefined' && window.ENV?.API_URL) {
        return window.ENV.API_URL;
    }

    // Fallback pour SSR ou si window.ENV n'est pas défini
    return process.env.NEXT_PUBLIC_API_URL || 'https://qcm.mirandaandcie.fr/api';
}
