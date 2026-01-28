#!/bin/sh
set -e

# Générer le fichier de configuration au démarrage
cat > /app/public/config.js << EOF
window.ENV = {
  API_URL: '${NEXT_PUBLIC_API_URL:-https://qcm.mirandaandcie.fr/api}'
};
EOF

echo "Configuration générée:"
cat /app/public/config.js

# Exécuter la commande principale
exec "$@"
