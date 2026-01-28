import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
    if (request.nextUrl.pathname === '/config.js') {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'https://qcm.mirandaandcie.fr/api';

        return new NextResponse(
            `window.ENV = { API_URL: '${apiUrl}' };`,
            {
                status: 200,
                headers: {
                    'Content-Type': 'application/javascript',
                    'Cache-Control': 'no-cache, no-store, must-revalidate',
                },
            }
        );
    }
}

export const config = {
    matcher: '/config.js',
};
