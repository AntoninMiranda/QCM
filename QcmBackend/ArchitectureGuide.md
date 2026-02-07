# Guide d'Architecture et de Codage pour QcmBackend

Ce document décrit l'architecture logicielle et les conventions de codage utilisées dans le projet QcmBackend, un backend ASP.NET Core basé sur l'architecture propre (Clean Architecture). Il est destiné à guider les développeurs, y compris les IA, pour maintenir la cohérence et la qualité du code.

## Vue d'ensemble de l'Architecture

Le projet suit les principes de l'architecture propre avec séparation des préoccupations en couches distinctes :

- **Domain** : Logique métier, entités, objets de valeur, événements
- **Application** : Cas d'usage, commandes, requêtes, gestion des erreurs
- **Infrastructure** : Accès aux données, services externes, identité
- **API** : Contrôleurs, middlewares, configuration HTTP

### Flux de Données
```
API → Application → Domain → Infrastructure
     ↓
   Result/Error
```

## Structure des Dossiers

```
src/
├── API/                    # Couche présentation
│   ├── Controllers/        # Contrôleurs API
│   ├── Common/             # Middlewares, extensions, attributs
│   ├── Requests/           # DTOs de requête
│   └── Program.cs          # Configuration de l'application
├── Application/            # Couche application
│   ├── Features/           # Fonctionnalités organisées par domaine
│   ├── Common/             # Interfaces, comportements, résultats
│   └── ConfigureServices.cs
├── Domain/                 # Couche domaine
│   ├── Entities/           # Entités métier
│   ├── ValueObjects/       # Objets de valeur
│   ├── Events/             # Événements domaine
│   └── Common/             # Classes de base
└── Infrastructure/         # Couche infrastructure
    ├── Data/               # Contexte EF, migrations
    ├── Identity/           # Gestion des utilisateurs
    ├── Services/           # Services externes
    └── ConfigureServices.cs
```

## Patterns et Conventions

### 1. CQRS avec MediatR

Toutes les opérations métier utilisent le pattern CQRS via MediatR.

**Structure d'une Commande :**
```
Features/{Feature}/Commands/{CommandName}/
├── {CommandName}.cs              # Définition de la commande
├── {CommandName}Handler.cs       # Gestionnaire
└── {CommandName}Validator.cs     # Validateur FluentValidation
```

**Exemple :**
```csharp
// Command
public class LoginCommand : IRequest<Result<ReadTokenDto>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool RememberMe { get; init; }
}

// Handler
public class LoginCommandHandler(IIdentityService identityService) 
    : IRequestHandler<LoginCommand, Result<ReadTokenDto>>
{
    public async Task<Result<ReadTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        return await identityService.LoginAsync(request, cancellationToken);
    }
}

// Validator
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .EmailAddress().WithErrorCode(ValidationCodes.Invalid);
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ValidationCodes.Required);
    }
}
```

### 2. Pattern Result pour la Gestion d'Erreurs

Utilisez le pattern Result pour encapsuler les succès et échecs.

```csharp
// Résultat générique
public class Result<TValue> : Result
{
    // Implémentation avec Value et Error
}

// Utilisation
Result<ReadTokenDto> result = await mediator.Send(command);
return result.Match(
    onSuccess: Ok,
    onFailure: Problem
);
```

**Types d'Erreurs :**
```csharp
public enum ErrorType
{
    Failure = 0,
    NotFound = 1,
    Validation = 2,
    Conflict = 3,
    AccessUnAuthorized = 4,
    AccessForbidden = 5
}
```

### 3. Entités du Domaine

**Classes de Base :**
- `BaseEntity` : Entité simple avec Id GUID et événements domaine
- `BaseAuditableEntity` : Avec audit (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- `BaseSoftDeleteEntity` : Avec suppression logique

**Exemple :**
```csharp
public class User : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Quiz> CreatedQuizzes { get; set; } = [];
}
```

### 4. Contrôleurs API

**Structure :**
- Héritent de `BaseController`
- Utilisent AutoMapper pour mapper les requêtes
- Retournent des `Result<T>` ou `Result`
- Gestion centralisée des erreurs via `Problem(Error)`

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginCommand command = mapper.Map<LoginCommand>(request);
        Result<ReadTokenDto> result = await mediator.Send(command);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
}
```

### 5. Validation

- Utilise FluentValidation
- Codes d'erreur standardisés dans `ValidationCodes`
- Validation automatique via `ValidationBehavior`

### 6. Mapping

- AutoMapper pour tous les mappings
- Profiles dans chaque feature ou globalement

### 7. Accès aux Données

**Entity Framework :**
- `AppDbContext` avec intercepteurs pour audit, soft delete, événements domaine
- Interfaces : `IAppDbContext`
- Migrations organisées

**Intercepteurs :**
- `AuditableEntityInterceptor`
- `SoftDeleteInterceptor`
- `DispatchDomainEventsInterceptor`

### 8. Services d'Infrastructure

- Interfaces dans `Application.Common.Interfaces`
- Implémentations dans `Infrastructure.Services`
- Injection de dépendances

### 9. Événements Domaine

- Héritent de `BaseEvent`
- Dispatchés automatiquement via intercepteur EF
- Gestionnaires dans `Application.EventHandlers`

## Conventions de Codage

### C# Moderne
- C# 12 avec constructeurs primaires
- `required` pour les propriétés obligatoires
- `init` pour les propriétés immuables
- `using` global pour les namespaces communs

### Nommage
- PascalCase pour classes, méthodes, propriétés
- camelCase pour paramètres, variables locales
- Interfaces préfixées par `I`
- DTOs préfixés par `Read`, `Create`, `Update`

### Async/Await
- Toutes les opérations I/O sont async
- `CancellationToken` passé dans les handlers
- `ConfigureAwait(false)` pour les bibliothèques

### Injection de Dépendances
- Constructeurs primaires pour DI
- Interfaces pour tous les services
- Scoped par défaut

### Gestion d'Erreurs
- Pas d'exceptions métier, utiliser `Result.Failure`
- Logging via `ILogger`
- Middleware d'exception globale

### Tests
- Tests unitaires pour handlers et services
- Tests d'intégration pour contrôleurs
- Utiliser `TestContainers` pour la DB

## Configuration et Démarrage

### Program.cs
- Configuration centralisée
- Services enregistrés par couche
- Middlewares dans l'ordre correct

### appsettings.json
- Configuration par environnement
- Secrets séparés

### Docker
- Multi-stage build
- Compose pour développement

## Outils et Bibliothèques

- **MediatR** : CQRS
- **FluentValidation** : Validation
- **AutoMapper** : Mapping
- **Entity Framework Core** : ORM
- **ASP.NET Core Identity** : Authentification
- **Serilog** : Logging
- **Swagger/OpenAPI** : Documentation API
- **xUnit** : Tests

## Bonnes Pratiques

1. **Séparation des Préoccupations** : Chaque couche a une responsabilité claire
2. **Dépendances** : Couches externes dépendent des internes via interfaces
3. **Immuabilité** : Préférer les objets immuables
4. **Validation** : Valider tôt, échouer vite
5. **Logging** : Logger les opérations importantes
6. **Tests** : Couverture élevée, tests automatisés
7. **Documentation** : Code auto-documenté, commentaires quand nécessaire

## Exemple Complet : Création d'une Nouvelle Feature

Pour ajouter une feature "Quiz" :

1. **Domain** : Créer `Quiz.cs` dans `Domain/Entities/`
2. **Application** : 
   - Créer dossier `Features/Quiz/`
   - Ajouter commandes/queries avec handlers et validators
3. **Infrastructure** : Ajouter configurations EF si nécessaire
4. **API** : Créer `QuizController.cs` avec endpoints
5. **Tests** : Ajouter tests pour chaque composant

Suivre ce guide assure la cohérence et maintenabilité du code.</content>
<filePath">/Volumes/DevSSD/code/pro/QCM/QcmBackend/ArchitectureGuide.md
