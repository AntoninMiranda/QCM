import type { NextConfig } from "next";

const nextConfig: NextConfig = {
    reactStrictMode: true,

    // Output standalone pour Docker
    output: 'standalone',

    // Désactiver les variables d'environnement au build time
    env: {},

    // Configuration pour servir config.js
    async rewrites() {
        return [
            {
                source: '/config.js',
                destination: '/api/config',
            },
        ];
    },

    // Headers de sécurité
    async headers() {
        return [
            {
                source: '/(.*)',
                headers: [
                    {
                        key: 'X-Frame-Options',
                        value: 'SAMEORIGIN',
                    },
                    {
                        key: 'X-Content-Type-Options',
                        value: 'nosniff',
                    },
                    {
                        key: 'Referrer-Policy',
                        value: 'strict-origin-when-cross-origin',
                    },
                ],
            },
        ];
    },
};

export default nextConfig;
