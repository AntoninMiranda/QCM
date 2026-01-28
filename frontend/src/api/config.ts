import type { NextApiRequest, NextApiResponse } from 'next';

export default function handler(req: NextApiRequest, res: NextApiResponse) {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'https://qcm.mirandaandcie.fr/api';

    res.setHeader('Content-Type', 'application/javascript');
    res.setHeader('Cache-Control', 'no-cache, no-store, must-revalidate');

    res.status(200).send(`
window.ENV = {
  API_URL: '${apiUrl}'
};
  `);
}
