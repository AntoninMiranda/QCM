namespace WebApplication1.Models;


public class QcmCategory
{
    public Dictionary<string, List<List<QcmQuestion>>> Categories { get; set; }
}

public class QcmQuestion
{
    public int Id { get; set; }
    public string Annexe { get; set; }
    public string Question { get; set; }
    public Dictionary<string, string> Choices { get; set; }
    public string Answer { get; set; } 
}