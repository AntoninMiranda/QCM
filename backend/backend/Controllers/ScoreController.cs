using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScoreController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Sauvegarder un nouveau score de QCM
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ScoreResponseDto>> SaveScore(SaveScoreDto scoreDto)
    {
        var userId = GetUserIdFromToken();

        // Calcul du pourcentage
        double percentage = (double)scoreDto.CorrectAnswers / scoreDto.TotalQuestions * 100;

        var score = new Score
        {
            UserId = userId,
            QcmType = scoreDto.QcmType,
            Category = scoreDto.Category,
            TotalQuestions = scoreDto.TotalQuestions,
            CorrectAnswers = scoreDto.CorrectAnswers,
            IncorrectAnswers = scoreDto.IncorrectAnswers,
            Percentage = Math.Round(percentage, 2),
            TimeSpentSeconds = scoreDto.TimeSpentSeconds,
            Details = scoreDto.Details,
            CompletedAt = DateTime.UtcNow
        };

        _context.Scores.Add(score);
        await _context.SaveChangesAsync();

        return Ok(MapToScoreResponseDto(score));
    }

    /// <summary>
    /// Récupérer tous les scores de l'utilisateur connecté
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ScoreResponseDto>>> GetMyScores(
        [FromQuery] string? qcmType = null,
        [FromQuery] string? category = null,
        [FromQuery] int limit = 50)
    {
        var userId = GetUserIdFromToken();

        var query = _context.Scores
            .Where(s => s.UserId == userId);

        if (!string.IsNullOrEmpty(qcmType))
        {
            query = query.Where(s => s.QcmType == qcmType);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }

        var scores = await query
            .OrderByDescending(s => s.CompletedAt)
            .Take(limit)
            .ToListAsync();

        return Ok(scores.Select(MapToScoreResponseDto).ToList());
    }

    /// <summary>
    /// Récupérer les statistiques globales de l'utilisateur
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<UserStatsDto>> GetMyStats()
    {
        var userId = GetUserIdFromToken();

        var scores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        if (!scores.Any())
        {
            return Ok(new UserStatsDto());
        }

        var stats = new UserStatsDto
        {
            TotalQcmsCompleted = scores.Count,
            AverageScore = Math.Round(scores.Average(s => s.Percentage), 2),
            TotalQuestionsAnswered = scores.Sum(s => s.TotalQuestions),
            TotalCorrectAnswers = scores.Sum(s => s.CorrectAnswers),
            TotalTimeSpentSeconds = scores.Sum(s => s.TimeSpentSeconds),
            RecentScores = scores
                .OrderByDescending(s => s.CompletedAt)
                .Take(10)
                .Select(MapToScoreResponseDto)
                .ToList()
        };

        // Statistiques par catégorie
        var categoryGroups = scores
            .Where(s => !string.IsNullOrEmpty(s.Category))
            .GroupBy(s => s.Category);

        foreach (var group in categoryGroups)
        {
            var categoryScores = group.ToList();
            stats.CategoryStats[group.Key!] = new CategoryStatsDto
            {
                Category = group.Key!,
                QcmsCompleted = categoryScores.Count,
                AverageScore = Math.Round(categoryScores.Average(s => s.Percentage), 2),
                TotalQuestions = categoryScores.Sum(s => s.TotalQuestions),
                TotalCorrect = categoryScores.Sum(s => s.CorrectAnswers)
            };
        }

        return Ok(stats);
    }

    /// <summary>
    /// Récupérer les données pour un graphique d'évolution
    /// </summary>
    [HttpGet("graph")]
    public async Task<ActionResult<ScoreGraphDataDto>> GetGraphData(
        [FromQuery] string? qcmType = null,
        [FromQuery] string? category = null,
        [FromQuery] int days = 30)
    {
        var userId = GetUserIdFromToken();
        var startDate = DateTime.UtcNow.AddDays(-days);

        var query = _context.Scores
            .Where(s => s.UserId == userId && s.CompletedAt >= startDate);

        if (!string.IsNullOrEmpty(qcmType))
        {
            query = query.Where(s => s.QcmType == qcmType);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }

        var scores = await query
            .OrderBy(s => s.CompletedAt)
            .ToListAsync();

        var graphData = new ScoreGraphDataDto
        {
            Dates = scores.Select(s => s.CompletedAt.ToString("yyyy-MM-dd HH:mm")).ToList(),
            Scores = scores.Select(s => s.Percentage).ToList(),
            Categories = scores.Select(s => s.Category ?? "N/A").ToList()
        };

        return Ok(graphData);
    }

    /// <summary>
    /// Récupérer un score spécifique par ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ScoreResponseDto>> GetScore(int id)
    {
        var userId = GetUserIdFromToken();

        var score = await _context.Scores
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (score == null)
        {
            return NotFound(new { message = "Score non trouvé" });
        }

        return Ok(MapToScoreResponseDto(score));
    }

    /// <summary>
    /// Supprimer un score
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScore(int id)
    {
        var userId = GetUserIdFromToken();

        var score = await _context.Scores
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (score == null)
        {
            return NotFound(new { message = "Score non trouvé" });
        }

        _context.Scores.Remove(score);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Score supprimé avec succès" });
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private ScoreResponseDto MapToScoreResponseDto(Score score)
    {
        return new ScoreResponseDto
        {
            Id = score.Id,
            QcmType = score.QcmType,
            Category = score.Category,
            TotalQuestions = score.TotalQuestions,
            CorrectAnswers = score.CorrectAnswers,
            IncorrectAnswers = score.IncorrectAnswers,
            Percentage = score.Percentage,
            TimeSpentSeconds = score.TimeSpentSeconds,
            CompletedAt = score.CompletedAt
        };
    }
}

