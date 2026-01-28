using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configuration de l'authentification JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers();

// Configuration de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "QCM API",
        Version = "v1",
        Description = "API pour gérer les QCM avec authentification JWT"
    });

    // Configuration pour supporter JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Migration automatique de la base de données au démarrage
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Retry logic pour attendre que PostgreSQL soit prêt
    var maxRetries = 15;
    var delay = TimeSpan.FromSeconds(3);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation($"Tentative de connexion à la base de données ({i + 1}/{maxRetries})...");
            
            // Test de connexion
            if (dbContext.Database.CanConnect())
            {
                logger.LogInformation("Connexion à la base de données réussie");
                
                // Appliquer les migrations
                dbContext.Database.Migrate();
                logger.LogInformation("✅ Migrations appliquées avec succès");
                break;
            }
            else
            {
                logger.LogWarning("Impossible de se connecter à la base de données");
                if (i == maxRetries - 1)
                {
                    throw new Exception("Impossible de se connecter à PostgreSQL après " + maxRetries + " tentatives");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Erreur lors de la tentative {i + 1}/{maxRetries}: {ex.Message}");
            
            if (i == maxRetries - 1)
            {
                logger.LogError("❌ Échec de la connexion après toutes les tentatives");
                throw;
            }
            
            logger.LogInformation($"⏳ Nouvelle tentative dans {delay.TotalSeconds} secondes...");
            Thread.Sleep(delay);
        }
    }
}


// Configuration de Swagger UI
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "QCM API V1");
    c.RoutePrefix = "swagger"; // URL: http://localhost:5000/api/swagger
});

// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();