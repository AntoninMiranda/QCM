# Script pour nettoyer complètement l'environnement de développement
Write-Host "🧹 Nettoyage complet de l'environnement de DÉVELOPPEMENT..." -ForegroundColor Yellow

# Arrêter les conteneurs
docker-compose -f docker-compose.dev.yml --env-file .env.dev down

# Supprimer les volumes
Write-Host "🗑️  Suppression des volumes..." -ForegroundColor Yellow
docker volume rm qcm-postgres-data-dev -f 2>$null

# Supprimer le réseau
Write-Host "🌐 Suppression du réseau..." -ForegroundColor Yellow
docker network rm qcm-network-dev -f 2>$null

# Nettoyer les images non utilisées (optionnel)
$response = Read-Host "Voulez-vous également supprimer les images Docker non utilisées? (o/n)"
if ($response -eq "o" -or $response -eq "O" -or $response -eq "oui") {
    docker image prune -f
}

Write-Host ""
Write-Host "✅ Environnement de développement nettoyé!" -ForegroundColor Green
Write-Host "💡 Vous pouvez redémarrer avec: .\start-dev.ps1" -ForegroundColor Cyan

