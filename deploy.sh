#!/bin/bash

# Script de déploiement pour le serveur de production
# Usage: ./deploy.sh

echo "🚀 Déploiement de l'application QCM..."

# Arrêter les conteneurs existants
echo "⏹️  Arrêt des conteneurs..."
docker-compose down

# Supprimer les anciennes images du frontend (pour forcer le rebuild avec les nouvelles variables)
echo "🗑️  Suppression de l'ancienne image frontend..."
docker rmi qcm-frontend 2>/dev/null || true

# Rebuilder les images (surtout le frontend avec la nouvelle variable d'environnement)
echo "🔨 Build des images Docker..."
docker-compose build --no-cache frontend

# Démarrer les conteneurs
echo "▶️  Démarrage des conteneurs..."
docker-compose up -d

# Attendre que les services soient prêts
echo "⏳ Attente du démarrage des services..."
sleep 10

# Vérifier le statut
echo ""
echo "📊 Statut des conteneurs:"
docker-compose ps

echo ""
echo "✅ Déploiement terminé!"
echo ""
echo "🌐 Accès:"
echo "   Frontend: http://localhost:3000"
echo "   Backend:  http://localhost:5000"
echo ""
echo "📝 Logs:"
echo "   docker-compose logs -f"

