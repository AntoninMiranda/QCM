using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class SaveScoreDto
{
    [Required]
    public string QcmType { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int TotalQuestions { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int CorrectAnswers { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int IncorrectAnswers { get; set; }
    
    [Range(0, int.MaxValue)]
    public int TimeSpentSeconds { get; set; }
    
    public string? Details { get; set; }
}

public class ScoreResponseDto
{
    public int Id { get; set; }
    public string QcmType { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public double Percentage { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class UserStatsDto
{
    public int TotalQcmsCompleted { get; set; }
    public double AverageScore { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public int TotalCorrectAnswers { get; set; }
    public int TotalTimeSpentSeconds { get; set; }
    public List<ScoreResponseDto> RecentScores { get; set; } = new();
    public Dictionary<string, CategoryStatsDto> CategoryStats { get; set; } = new();
}

public class CategoryStatsDto
{
    public string Category { get; set; } = string.Empty;
    public int QcmsCompleted { get; set; }
    public double AverageScore { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalCorrect { get; set; }
}

public class ScoreGraphDataDto
{
    public List<string> Dates { get; set; } = new();
    public List<double> Scores { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}

