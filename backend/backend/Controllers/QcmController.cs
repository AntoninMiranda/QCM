using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Text.Json;

namespace WebApplication1.Controllers;


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

        var json = System.IO.File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<Dictionary<string, List<QcmQuestion>>>(json, options);

        if (data == null || data.Count == 0)
            return NotFound("Aucune question disponible.");

        List<QcmQuestion> questions;
        if (!string.IsNullOrEmpty(categorie) && data.ContainsKey(categorie))
            questions = data[categorie];
        else
            questions = data.Values.SelectMany(q => q).ToList();

        if (questions.Count == 0)
            return NotFound("Aucune question pour cette catégorie.");

        if (count.HasValue && count.Value > 0)
            questions = questions.OrderBy(x => Guid.NewGuid()).Take(count.Value).ToList();

        return Ok(questions);
    }
}