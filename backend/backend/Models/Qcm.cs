namespace backend.Models;

public class QcmCategory
{
    public required Dictionary<string, List<QcmQuestionGroup>> Categories { get; set; }
}

public class QcmQuestionGroup
{
    public required string Annexes { get; set; }
    public required List<QcmQuestion> Questions { get; set; }
}

public class QcmQuestion
{
    public int Id { get; set; }
    public required string Question { get; set; }
    public required Dictionary<string, string> Choices { get; set; }
    public required string Answer { get; set; } 
}