using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalQcmsCompleted { get; set; }
    public double AverageScore { get; set; }
}

public class UpdateUserByAdminDto
{
    [MaxLength(100)]
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
    
    public string? NewPassword { get; set; }
}

public class UserListDto
{
    public List<AdminUserDto> Users { get; set; } = new();
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalRegularUsers { get; set; }
}

