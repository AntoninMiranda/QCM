# Script pour créer un compte admin

param(
    [Parameter(Mandatory=$false)]
    [string]$Email = "admin@qcm.com",
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "admin",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "Admin123!"
)

Write-Host "🔧 Création d'un compte administrateur..." -ForegroundColor Cyan
Write-Host ""

# Vérifier si Docker est en cours d'exécution
$dockerRunning = docker ps 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker n'est pas en cours d'exécution" -ForegroundColor Red
    exit 1
}

# Vérifier que PostgreSQL est accessible
Write-Host "⏳ Vérification de PostgreSQL..." -ForegroundColor Yellow
$pgReady = docker-compose exec -T postgres pg_isready -U qcmuser 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ PostgreSQL n'est pas accessible" -ForegroundColor Red
    Write-Host "   Exécutez d'abord: docker-compose up -d" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ PostgreSQL est prêt" -ForegroundColor Green
Write-Host ""

# Demander confirmation
Write-Host "Création d'un compte admin avec les informations suivantes:" -ForegroundColor Cyan
Write-Host "  Email:    $Email" -ForegroundColor White
Write-Host "  Username: $Username" -ForegroundColor White
Write-Host "  Password: $Password" -ForegroundColor White
Write-Host ""

$confirmation = Read-Host "Continuer? (o/n)"
if ($confirmation -ne "o" -and $confirmation -ne "O") {
    Write-Host "❌ Opération annulée" -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "📝 Enregistrement via l'API..." -ForegroundColor Cyan

# Créer le compte via l'API
$body = @{
    username = $Username
    email = $Email
    password = $Password
    firstName = "Admin"
    lastName = "User"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/register" `
        -Method POST `
        -Body $body `
        -ContentType "application/json" `
        -ErrorAction Stop
    
    Write-Host "✅ Compte créé avec succès!" -ForegroundColor Green
    $userId = $response.user.id
    Write-Host "   User ID: $userId" -ForegroundColor White
}
catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️  L'utilisateur existe peut-être déjà" -ForegroundColor Yellow
        Write-Host "   On va essayer de le promouvoir en admin..." -ForegroundColor Yellow
    }
    else {
        Write-Host "❌ Erreur lors de la création du compte: $_" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "🔑 Promotion en administrateur..." -ForegroundColor Cyan

# Promouvoir en admin via SQL
$sqlCommand = "UPDATE \`"Users\`" SET \`"Role\`" = 'Admin' WHERE \`"Email\`" = '$Email';"

docker-compose exec -T postgres psql -U qcmuser -d qcmdb -c $sqlCommand 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Utilisateur promu en administrateur!" -ForegroundColor Green
}
else {
    Write-Host "❌ Erreur lors de la promotion" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🎉 Configuration terminée!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Informations de connexion:" -ForegroundColor Cyan
Write-Host "  URL:      http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  Email:    $Email" -ForegroundColor White
Write-Host "  Password: $Password" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  N'oubliez pas de changer le mot de passe après la première connexion!" -ForegroundColor Yellow
Write-Host ""

# Test de connexion
Write-Host "🧪 Test de connexion..." -ForegroundColor Cyan

$loginBody = @{
    email = $Email
    password = $Password
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
        -Method POST `
        -Body $loginBody `
        -ContentType "application/json" `
        -ErrorAction Stop
    
    Write-Host "✅ Connexion réussie!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎫 Token JWT (copier pour Swagger):" -ForegroundColor Cyan
    Write-Host "Bearer $($loginResponse.token)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "👤 Rôle: $($loginResponse.user.role)" -ForegroundColor Green
}
catch {
    Write-Host "⚠️  La connexion a échoué, mais le compte a été créé" -ForegroundColor Yellow
    Write-Host "   Réessayez de vous connecter manuellement" -ForegroundColor Yellow
}

