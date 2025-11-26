using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Text.Json;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
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
        var categoryData = JsonSerializer.Deserialize<Dictionary<string, List<List<QcmQuestion>>>>(jsonContent, options);

        if (categoryData == null || !categoryData.Any())
            return NotFound("Aucune donnée de QCM trouvée.");

        var random = new Random();
        List<List<QcmQuestion>> allQuestionLists = new List<List<QcmQuestion>>();

        // Si catégorie spécifiée, utiliser les listes de cette catégorie
        if (!string.IsNullOrEmpty(categorie) && categoryData.ContainsKey(categorie))
        {
            allQuestionLists = categoryData[categorie];
        }
        // Sinon, collecter toutes les listes de questions de toutes les catégories
        else
        {
            foreach (var category in categoryData.Keys)
            {
                allQuestionLists.AddRange(categoryData[category]);
            }
        }

        if (allQuestionLists.Count == 0)
            return NotFound("Aucune question disponible.");

        // Mélanger toutes les listes de questions
        allQuestionLists = allQuestionLists.OrderBy(q => random.Next()).ToList();

        if (!count.HasValue)
        {
            // Sans count spécifié: retourner toutes les listes de questions mélangées
            return Ok(allQuestionLists);
        }
        else
        {
            // Avec count spécifié: sélectionner count listes aléatoirement
            int numListsToSelect = Math.Min(count.Value, allQuestionLists.Count);
            List<List<QcmQuestion>> selectedLists = allQuestionLists.Take(numListsToSelect).ToList();
            return Ok(selectedLists);
        }
    }
}