using Microsoft.AspNetCore.Mvc;
using Backend.Data;

namespace Backend.Controllers;

/// <summary>
/// Exemple de contrôleur montrant comment utiliser la base de données
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatabaseTestController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public DatabaseTestController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Test de connexion à la base de données
    /// </summary>
    [HttpGet("test-connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Vérifie que la connexion à la base de données fonctionne
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                return Ok(new 
                { 
                    message = "Connexion à la base de données réussie !",
                    database = "PostgreSQL",
                    status = "connected"
                });
            }
            else
            {
                return StatusCode(500, new 
                { 
                    message = "Impossible de se connecter à la base de données",
                    status = "disconnected"
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Erreur lors de la connexion à la base de données",
                error = ex.Message,
                status = "error"
            });
        }
    }
    
    /// <summary>
    /// Obtient des informations sur la base de données
    /// </summary>
    [HttpGet("info")]
    public IActionResult GetDatabaseInfo()
    {
        try
        {
            var providerName = _context.Database.ProviderName;
            
            return Ok(new 
            { 
                provider = providerName,
                message = "Base de données PostgreSQL configurée"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Erreur lors de la récupération des informations",
                error = ex.Message
            });
        }
    }
}

