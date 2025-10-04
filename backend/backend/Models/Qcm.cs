namespace WebApplication1.Models;

public class QcmCategorie
{
    public List<QcmQuestion> Categorie { get; set; }
}
public class QcmQuestion
{
    public int Id { get; set; }
    public string Question { get; set; }
    public Dictionary<string, string> Choices { get; set; }
    public string Answer { get; set; } 
}