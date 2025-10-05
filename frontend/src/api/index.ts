// src/api/index.ts
const BASE_URL = 'http://api.mirandaandcie.fr';

export async function getData(endpoint: string) {
    let url: string = `${BASE_URL}/Qcm/${endpoint}`;
    console.log(url);
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error('Erreur lors de la récupération des données');
    }
    console.log(response);
    return response.json();
}

export async function postData(endpoint: string, data: any) {
    const response = await fetch(`${BASE_URL}/${endpoint}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    if (!response.ok) {
        throw new Error('Erreur lors de l\'envoi des données');
    }
    return response.json();
}
