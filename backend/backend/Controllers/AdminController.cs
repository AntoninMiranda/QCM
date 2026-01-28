using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Récupérer la liste de tous les utilisateurs (Admin uniquement)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<UserListDto>> GetAllUsers(
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Users.AsQueryable();

        // Filtre par recherche (username ou email)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => 
                u.Username.Contains(search) || 
                u.Email.Contains(search) ||
                (u.FirstName != null && u.FirstName.Contains(search)) ||
                (u.LastName != null && u.LastName.Contains(search))
            );
        }

        // Filtre par rôle
        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role == role);
        }

        var totalUsers = await query.CountAsync();
        
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userDtos = new List<AdminUserDto>();
        
        foreach (var user in users)
        {
            var scores = await _context.Scores
                .Where(s => s.UserId == user.Id)
                .ToListAsync();

            userDtos.Add(new AdminUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TotalQcmsCompleted = scores.Count,
                AverageScore = scores.Any() ? Math.Round(scores.Average(s => s.Percentage), 2) : 0
            });
        }

        var result = new UserListDto
        {
            Users = userDtos,
            TotalUsers = totalUsers,
            TotalAdmins = await _context.Users.CountAsync(u => u.Role == UserRoles.Admin),
            TotalRegularUsers = await _context.Users.CountAsync(u => u.Role == UserRoles.User)
        };

        return Ok(result);
    }

    /// <summary>
    /// Récupérer les détails d'un utilisateur spécifique (Admin uniquement)
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<AdminUserDto>> GetUserById(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        var scores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        var userDto = new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TotalQcmsCompleted = scores.Count,
            AverageScore = scores.Any() ? Math.Round(scores.Average(s => s.Percentage), 2) : 0
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Récupérer tous les scores d'un utilisateur (Admin uniquement)
    /// </summary>
    [HttpGet("users/{userId}/scores")]
    public async Task<ActionResult<List<ScoreResponseDto>>> GetUserScores(
        int userId,
        [FromQuery] int limit = 50)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        var scores = await _context.Scores
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CompletedAt)
            .Take(limit)
            .ToListAsync();

        var scoreDtos = scores.Select(s => new ScoreResponseDto
        {
            Id = s.Id,
            QcmType = s.QcmType,
            Category = s.Category,
            TotalQuestions = s.TotalQuestions,
            CorrectAnswers = s.CorrectAnswers,
            IncorrectAnswers = s.IncorrectAnswers,
            Percentage = s.Percentage,
            TimeSpentSeconds = s.TimeSpentSeconds,
            CompletedAt = s.CompletedAt
        }).ToList();

        return Ok(scoreDtos);
    }

    /// <summary>
    /// Modifier un utilisateur (Admin uniquement)
    /// </summary>
    [HttpPut("users/{userId}")]
    public async Task<ActionResult<AdminUserDto>> UpdateUser(int userId, UpdateUserByAdminDto updateDto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        // Vérifier si on change le username et s'il est disponible
        if (!string.IsNullOrEmpty(updateDto.Username) && updateDto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == updateDto.Username && u.Id != userId))
            {
                return BadRequest(new { message = "Ce nom d'utilisateur est déjà pris" });
            }
            user.Username = updateDto.Username;
        }

        // Vérifier si on change l'email et s'il est disponible
        if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == updateDto.Email && u.Id != userId))
            {
                return BadRequest(new { message = "Cet email est déjà utilisé" });
            }
            user.Email = updateDto.Email;
        }

        // Mise à jour des autres champs
        if (updateDto.FirstName != null)
        {
            user.FirstName = updateDto.FirstName;
        }

        if (updateDto.LastName != null)
        {
            user.LastName = updateDto.LastName;
        }

        // Mise à jour du rôle
        if (!string.IsNullOrEmpty(updateDto.Role))
        {
            if (updateDto.Role != UserRoles.Admin && updateDto.Role != UserRoles.User)
            {
                return BadRequest(new { message = "Rôle invalide. Utilisez 'Admin' ou 'User'" });
            }
            user.Role = updateDto.Role;
        }

        // Changement de mot de passe
        if (!string.IsNullOrEmpty(updateDto.NewPassword))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var scores = await _context.Scores.Where(s => s.UserId == userId).ToListAsync();

        var userDto = new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TotalQcmsCompleted = scores.Count,
            AverageScore = scores.Any() ? Math.Round(scores.Average(s => s.Percentage), 2) : 0
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Supprimer un utilisateur (Admin uniquement)
    /// </summary>
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        // Empêcher la suppression de son propre compte
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == currentUserId)
        {
            return BadRequest(new { message = "Vous ne pouvez pas supprimer votre propre compte" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Utilisateur supprimé avec succès" });
    }

    /// <summary>
    /// Promouvoir un utilisateur en Admin (Admin uniquement)
    /// </summary>
    [HttpPost("users/{userId}/promote")]
    public async Task<ActionResult<AdminUserDto>> PromoteToAdmin(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        if (user.Role == UserRoles.Admin)
        {
            return BadRequest(new { message = "Cet utilisateur est déjà administrateur" });
        }

        user.Role = UserRoles.Admin;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var scores = await _context.Scores.Where(s => s.UserId == userId).ToListAsync();

        var userDto = new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TotalQcmsCompleted = scores.Count,
            AverageScore = scores.Any() ? Math.Round(scores.Average(s => s.Percentage), 2) : 0
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Rétrograder un Admin en User (Admin uniquement)
    /// </summary>
    [HttpPost("users/{userId}/demote")]
    public async Task<ActionResult<AdminUserDto>> DemoteFromAdmin(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        if (user.Role == UserRoles.User)
        {
            return BadRequest(new { message = "Cet utilisateur n'est pas administrateur" });
        }

        // Empêcher de se rétrograder soi-même
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == currentUserId)
        {
            return BadRequest(new { message = "Vous ne pouvez pas vous rétrograder vous-même" });
        }

        user.Role = UserRoles.User;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var scores = await _context.Scores.Where(s => s.UserId == userId).ToListAsync();

        var userDto = new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TotalQcmsCompleted = scores.Count,
            AverageScore = scores.Any() ? Math.Round(scores.Average(s => s.Percentage), 2) : 0
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Supprimer un score spécifique d'un utilisateur (Admin uniquement)
    /// </summary>
    [HttpDelete("users/{userId}/scores/{scoreId}")]
    public async Task<IActionResult> DeleteUserScore(int userId, int scoreId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        var score = await _context.Scores
            .FirstOrDefaultAsync(s => s.Id == scoreId && s.UserId == userId);

        if (score == null)
        {
            return NotFound(new { message = "Score non trouvé pour cet utilisateur" });
        }

        _context.Scores.Remove(score);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Admin supprimé le score {scoreId} de l'utilisateur {userId}");

        return Ok(new { message = "Score supprimé avec succès" });
    }

    /// <summary>
    /// Supprimer plusieurs scores d'un utilisateur (Admin uniquement)
    /// </summary>
    [HttpPost("users/{userId}/scores/delete-multiple")]
    public async Task<IActionResult> DeleteMultipleUserScores(int userId, [FromBody] List<int> scoreIds)
    {
        if (scoreIds == null || !scoreIds.Any())
        {
            return BadRequest(new { message = "Aucun ID de score fourni" });
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        var scores = await _context.Scores
            .Where(s => scoreIds.Contains(s.Id) && s.UserId == userId)
            .ToListAsync();

        if (!scores.Any())
        {
            return NotFound(new { message = "Aucun score trouvé correspondant aux IDs fournis" });
        }

        var deletedCount = scores.Count;
        _context.Scores.RemoveRange(scores);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Admin supprimé {deletedCount} score(s) de l'utilisateur {userId}");

        return Ok(new 
        { 
            message = $"{deletedCount} score(s) supprimé(s) avec succès",
            deletedCount = deletedCount
        });
    }

    /// <summary>
    /// Supprimer tous les scores d'un utilisateur (Admin uniquement)
    /// </summary>
    [HttpDelete("users/{userId}/scores")]
    public async Task<IActionResult> DeleteAllUserScores(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        var scores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        if (!scores.Any())
        {
            return Ok(new { message = "L'utilisateur n'a aucun score à supprimer" });
        }

        var deletedCount = scores.Count;
        _context.Scores.RemoveRange(scores);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Admin supprimé tous les scores ({deletedCount}) de l'utilisateur {userId}");

        return Ok(new 
        { 
            message = $"Tous les scores de l'utilisateur ont été supprimés ({deletedCount} score(s))",
            deletedCount = deletedCount
        });
    }

    /// <summary>
    /// Obtenir des statistiques globales (Admin uniquement)
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult> GetStatistics()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalAdmins = await _context.Users.CountAsync(u => u.Role == UserRoles.Admin);
        var totalScores = await _context.Scores.CountAsync();
        var averageScore = await _context.Scores.AnyAsync() 
            ? await _context.Scores.AverageAsync(s => s.Percentage) 
            : 0;

        var recentUsers = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.Role,
                u.CreatedAt
            })
            .ToListAsync();

        var stats = new
        {
            TotalUsers = totalUsers,
            TotalAdmins = totalAdmins,
            TotalRegularUsers = totalUsers - totalAdmins,
            TotalScores = totalScores,
            AverageScore = Math.Round(averageScore, 2),
            RecentUsers = recentUsers
        };

        return Ok(stats);
    }
}

