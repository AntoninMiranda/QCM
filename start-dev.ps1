# Script pour démarrer l'environnement de développement
}
    docker-compose -f docker-compose.dev.yml --env-file .env.dev logs -f
if ($response -eq "o" -or $response -eq "O" -or $response -eq "oui") {
$response = Read-Host "Voulez-vous suivre les logs? (o/n)"
# Proposer de suivre les logs

Write-Host ""
Write-Host "  - Voir les conteneurs: docker-compose -f docker-compose.dev.yml ps" -ForegroundColor White
Write-Host "  - Redémarrer:         docker-compose -f docker-compose.dev.yml restart" -ForegroundColor White
Write-Host "  - Arrêter:            docker-compose -f docker-compose.dev.yml down" -ForegroundColor White
Write-Host "  - Voir les logs:      docker-compose -f docker-compose.dev.yml logs -f" -ForegroundColor White
Write-Host "📝 Commandes utiles:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  - PostgreSQL: localhost:5433" -ForegroundColor White
Write-Host "  - Swagger:  http://localhost:5079/swagger" -ForegroundColor White
Write-Host "  - Backend:  http://localhost:5079" -ForegroundColor White
Write-Host "  - Frontend: http://localhost:3001" -ForegroundColor White
Write-Host "📊 Services disponibles:" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Environnement de développement démarré!" -ForegroundColor Green
Write-Host ""
# Afficher les logs

docker-compose -f docker-compose.dev.yml --env-file .env.dev up --build -d
Write-Host "🔨 Construction et démarrage des conteneurs..." -ForegroundColor Green
# Construire et démarrer les conteneurs

# docker volume rm qcm-postgres-data-dev -f
# Write-Host "🗑️  Suppression des volumes de développement..." -ForegroundColor Yellow
# Supprimer les anciens volumes (optionnel - décommenter si besoin de repartir de zéro)

docker-compose -f docker-compose.dev.yml --env-file .env.dev down
Write-Host "📦 Arrêt des conteneurs de développement existants..." -ForegroundColor Yellow
# Arrêter les conteneurs existants

Write-Host "🚀 Démarrage de l'environnement de DÉVELOPPEMENT..." -ForegroundColor Cyan

