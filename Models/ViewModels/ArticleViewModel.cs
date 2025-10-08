namespace NewsPortal.Models.ViewModels;

public class ArticleViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImagePath { get; set; }
}
