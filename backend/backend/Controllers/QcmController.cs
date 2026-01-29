using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.Text.Json;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QcmController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] string type, [FromQuery] string? categorie = null, [FromQuery] int? count = null)
    {
        if (string.IsNullOrEmpty(type))
            return BadRequest("Le paramètre 'type' est requis.");

        var jsonPath = $"Data/{type}.json";
        if (!System.IO.File.Exists(jsonPath))
            return NotFound("Fichier JSON non trouvé.");

        string jsonContent = System.IO.File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var categoryData = JsonSerializer.Deserialize<Dictionary<string, List<QcmQuestionGroup>>>(jsonContent, options);

        if (categoryData == null || !categoryData.Any())
            return NotFound("Aucune donnée de QCM trouvée.");

        var random = new Random();
        List<QcmQuestionGroup> allQuestionGroups = new List<QcmQuestionGroup>();

        // Si catégorie spécifiée, utiliser les groupes de cette catégorie
        if (!string.IsNullOrEmpty(categorie) && categoryData.ContainsKey(categorie))
        {
            allQuestionGroups = categoryData[categorie];
        }
        // Sinon, collecter tous les groupes de questions de toutes les catégories
        else
        {
            foreach (var category in categoryData.Keys)
            {
                allQuestionGroups.AddRange(categoryData[category]);
            }
        }

        if (allQuestionGroups.Count == 0)
            return NotFound("Aucune question disponible.");

        // Mélanger tous les groupes de questions
        allQuestionGroups = allQuestionGroups.OrderBy(_ => random.Next()).ToList();

        if (!count.HasValue)
        {
            // Sans count spécifié: retourner tous les groupes de questions mélangés
            return Ok(allQuestionGroups);
        }
        else
        {
            // Avec count spécifié: sélectionner count groupes aléatoirement
            int numGroupsToSelect = Math.Min(count.Value, allQuestionGroups.Count);
            List<QcmQuestionGroup> selectedGroups = allQuestionGroups.Take(numGroupsToSelect).ToList();
            return Ok(selectedGroups);
        }
    }
}