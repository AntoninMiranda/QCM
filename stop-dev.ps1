# Script pour arrêter l'environnement de développement
Write-Host "🛑 Arrêt de l'environnement de DÉVELOPPEMENT..." -ForegroundColor Yellow

docker-compose -f docker-compose.dev.yml --env-file .env.dev down

Write-Host "✅ Environnement de développement arrêté!" -ForegroundColor Green

