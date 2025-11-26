# Script de déploiement PowerShell pour Windows
# Usage: .\deploy.ps1

Write-Host "🚀 Déploiement de l'application QCM..." -ForegroundColor Green

# Arrêter les conteneurs existants
Write-Host "⏹️  Arrêt des conteneurs..." -ForegroundColor Yellow
docker-compose down

# Supprimer les anciennes images du frontend
Write-Host "🗑️  Suppression de l'ancienne image frontend..." -ForegroundColor Yellow
docker rmi qcm-frontend 2>$null

# Rebuilder les images
Write-Host "🔨 Build des images Docker..." -ForegroundColor Yellow
docker-compose build --no-cache frontend

# Démarrer les conteneurs
Write-Host "▶️  Démarrage des conteneurs..." -ForegroundColor Yellow
docker-compose up -d

# Attendre que les services soient prêts
Write-Host "⏳ Attente du démarrage des services..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Vérifier le statut
Write-Host ""
Write-Host "📊 Statut des conteneurs:" -ForegroundColor Cyan
docker-compose ps

Write-Host ""
Write-Host "✅ Déploiement terminé!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Accès:" -ForegroundColor Cyan
Write-Host "   Frontend: http://localhost:3000"
Write-Host "   Backend:  http://localhost:5000"
Write-Host ""
Write-Host "📝 Logs:" -ForegroundColor Cyan
Write-Host "   docker-compose logs -f"

