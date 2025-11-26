using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Score
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string QcmType { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [Required]
    public int TotalQuestions { get; set; }
    
    [Required]
    public int CorrectAnswers { get; set; }
    
    [Required]
    public int IncorrectAnswers { get; set; }
    
    public double Percentage { get; set; }
    
    public int TimeSpentSeconds { get; set; }
    
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    // Détails supplémentaires stockés en JSON
    public string? Details { get; set; }
}

