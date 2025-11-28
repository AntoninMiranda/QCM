# Script pour démarrer l'environnement de production
Write-Host "🚀 Démarrage de l'environnement de PRODUCTION..." -ForegroundColor Cyan

# Arrêter les conteneurs existants
Write-Host "📦 Arrêt des conteneurs de production existants..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml --env-file .env.prod down

# Construire et démarrer les conteneurs
Write-Host "🔨 Construction et démarrage des conteneurs..." -ForegroundColor Green
docker-compose -f docker-compose.yml --env-file .env.prod up --build -d

# Afficher les logs
Write-Host ""
Write-Host "✅ Environnement de production démarré!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Services disponibles:" -ForegroundColor Cyan
Write-Host "  - Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "  - Backend:  http://localhost:5000" -ForegroundColor White
Write-Host "  - Swagger:  http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - PostgreSQL: localhost:5432" -ForegroundColor White
Write-Host ""
Write-Host "📝 Commandes utiles:" -ForegroundColor Cyan
Write-Host "  - Voir les logs:      docker-compose logs -f" -ForegroundColor White
Write-Host "  - Arrêter:            docker-compose down" -ForegroundColor White
Write-Host "  - Redémarrer:         docker-compose restart" -ForegroundColor White
Write-Host "  - Voir les conteneurs: docker-compose ps" -ForegroundColor White
Write-Host ""

# Proposer de suivre les logs
$response = Read-Host "Voulez-vous suivre les logs? (o/n)"
if ($response -eq "o" -or $response -eq "O" -or $response -eq "oui") {
    docker-compose -f docker-compose.yml --env-file .env.prod logs -f
}

