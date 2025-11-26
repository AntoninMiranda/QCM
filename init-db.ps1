# Script pour initialiser la base de données PostgreSQL

Write-Host "🔧 Initialisation de la base de données..." -ForegroundColor Cyan

# Vérifier si Docker est en cours d'exécution
$dockerRunning = docker ps 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker n'est pas en cours d'exécution" -ForegroundColor Red
    exit 1
}

# Attendre que PostgreSQL soit prêt
Write-Host "⏳ Attente que PostgreSQL soit prêt..." -ForegroundColor Yellow
$maxRetries = 30
$retryCount = 0

while ($retryCount -lt $maxRetries) {
    $result = docker-compose exec -T postgres pg_isready -U qcmuser 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL est prêt!" -ForegroundColor Green
        break
    }
    
    $retryCount++
    Write-Host "⏳ Tentative $retryCount/$maxRetries..." -ForegroundColor Yellow
    Start-Sleep -Seconds 2
}

if ($retryCount -eq $maxRetries) {
    Write-Host "❌ PostgreSQL n'est pas accessible après $maxRetries tentatives" -ForegroundColor Red
    exit 1
}

# Copier le fichier SQL dans le conteneur
Write-Host "📋 Copie du script SQL..." -ForegroundColor Cyan
docker cp create_users_table.sql qcm-postgres-1:/tmp/init.sql

# Exécuter le script SQL
Write-Host "🚀 Exécution du script SQL..." -ForegroundColor Cyan
docker-compose exec -T postgres psql -U qcmuser -d qcmdb -f /tmp/init.sql

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Base de données initialisée avec succès!" -ForegroundColor Green
    
    # Vérifier les tables créées
    Write-Host "`n📊 Tables créées:" -ForegroundColor Cyan
    docker-compose exec -T postgres psql -U qcmuser -d qcmdb -c "\dt"
} else {
    Write-Host "❌ Erreur lors de l'initialisation de la base de données" -ForegroundColor Red
    exit 1
}

# Redémarrer le backend pour appliquer les changements
Write-Host "`n🔄 Redémarrage du backend..." -ForegroundColor Cyan
docker-compose restart backend

Write-Host "`n✨ Configuration terminée!" -ForegroundColor Green
Write-Host "🌐 Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan

