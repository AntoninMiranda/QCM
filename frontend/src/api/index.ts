// src/api/index.ts
import { getApiUrl } from "@/lib/api";

const BASE_URL = getApiUrl();

export async function getData(parameter: string, token?: string) {
    let url: string = `${BASE_URL}/Qcm${parameter}`;
    console.log(url);

    const headers: HeadersInit = {};
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, {
        headers: headers
    });

    if (!response.ok) {
        if (response.status === 401) {
            throw new Error('Session expirée. Veuillez vous reconnecter.');
        }
        throw new Error('Erreur lors de la récupération des données');
    }
    console.log(response);
    return response.json();
}

export async function postData(endpoint: string, data: any, token?: string) {
    const headers: HeadersInit = {
        'Content-Type': 'application/json',
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${BASE_URL}/${endpoint}`, {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        if (response.status === 401) {
            throw new Error('Session expirée. Veuillez vous reconnecter.');
        }
        throw new Error('Erreur lors de l\'envoi des données');
    }
    return response.json();
}

// Auth functions
export const authApi = {
    register: async (data: {
        username: string;
        email: string;
        password: string;
        firstName?: string;
        lastName?: string;
    }) => {
        const response = await fetch(`${BASE_URL}/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de l\'inscription';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },

    login: async (data: {
        email: string;
        password: string
    }) => {
        const response = await fetch(`${BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de la connexion';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },

    getProfile: async (token: string) => {
        const response = await fetch(`${BASE_URL}/Auth/profile`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de la récupération du profil';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },

    updateProfile: async (token: string, data: {
        username?: string;
        email?: string;
        firstName?: string;
        lastName?: string;
        currentPassword?: string;
        newPassword?: string;
    }) => {
        const response = await fetch(`${BASE_URL}/Auth/profile`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de la mise à jour du profil';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },
};

// Score functions
export const scoreApi = {
    saveScore: async (token: string, data: {
        qcmType: string;
        category?: string;
        totalQuestions: number;
        correctAnswers: number;
        incorrectAnswers: number;
        timeSpentSeconds?: number;
        details?: string;
    }) => {
        const response = await fetch(`${BASE_URL}/Score`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de l\'enregistrement du score';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },

    getMyScores: async (token: string, filters?: {
        qcmType?: string;
        category?: string;
        limit?: number;
    }) => {
        let url = `${BASE_URL}/Score`;
        const params = new URLSearchParams();

        if (filters?.qcmType) params.append('qcmType', filters.qcmType);
        if (filters?.category) params.append('category', filters.category);
        if (filters?.limit) params.append('limit', filters.limit.toString());

        const queryString = params.toString();
        if (queryString) url += `?${queryString}`;

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de la récupération des scores';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },

    getMyStats: async (token: string) => {
        const response = await fetch(`${BASE_URL}/Score/stats`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        });

        if (!response.ok) {
            let errorMessage = 'Erreur lors de la récupération des statistiques';
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    errorMessage = error.message || errorMessage;
                }
            } catch (e) {
                // Ignore parsing error
            }
            throw new Error(errorMessage);
        }

        return response.json();
    },
};

