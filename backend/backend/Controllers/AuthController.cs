using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using BCrypt.Net;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        // Vérifier si l'email existe déjà
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return BadRequest(new { message = "Cet email est déjà utilisé" });
        }

        // Vérifier si le username existe déjà
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            return BadRequest(new { message = "Ce nom d'utilisateur est déjà pris" });
        }

        // Créer le nouvel utilisateur
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Générer le token JWT
        var token = GenerateJwtToken(user);

        var response = new AuthResponseDto
        {
            Token = token,
            User = MapToUserProfileDto(user)
        };

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        // Trouver l'utilisateur par email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email ou mot de passe incorrect" });
        }

        // Générer le token JWT
        var token = GenerateJwtToken(user);

        var response = new AuthResponseDto
        {
            Token = token,
            User = MapToUserProfileDto(user)
        };

        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = GetUserIdFromToken();
        
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        return Ok(MapToUserProfileDto(user));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(UpdateProfileDto updateDto)
    {
        var userId = GetUserIdFromToken();
        
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        // Vérifier si on veut changer le mot de passe
        if (!string.IsNullOrEmpty(updateDto.NewPassword))
        {
            if (string.IsNullOrEmpty(updateDto.CurrentPassword))
            {
                return BadRequest(new { message = "Le mot de passe actuel est requis pour changer le mot de passe" });
            }

            if (!BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Mot de passe actuel incorrect" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
        }

        // Mettre à jour les autres champs
        if (!string.IsNullOrEmpty(updateDto.Username) && updateDto.Username != user.Username)
        {
            // Vérifier si le nouveau username est disponible
            if (await _context.Users.AnyAsync(u => u.Username == updateDto.Username && u.Id != userId))
            {
                return BadRequest(new { message = "Ce nom d'utilisateur est déjà pris" });
            }
            user.Username = updateDto.Username;
        }

        if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
        {
            // Vérifier si le nouvel email est disponible
            if (await _context.Users.AnyAsync(u => u.Email == updateDto.Email && u.Id != userId))
            {
                return BadRequest(new { message = "Cet email est déjà utilisé" });
            }
            user.Email = updateDto.Email;
        }

        if (updateDto.FirstName != null)
        {
            user.FirstName = updateDto.FirstName;
        }

        if (updateDto.LastName != null)
        {
            user.LastName = updateDto.LastName;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToUserProfileDto(user));
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private UserProfileDto MapToUserProfileDto(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
    }
}

