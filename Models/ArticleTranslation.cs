using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Models;

public class ArticleTranslation
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Language { get; set; } = "ru"; 

    [Required(ErrorMessage = "TitleRequired")]
    [StringLength(200, ErrorMessage = "TitleMaxLength")]
    public string Title { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "SubtitleRequired")]
    [StringLength(300, ErrorMessage = "SubtitleMaxLength")]
    public string? Subtitle { get; set; }

    [Required(ErrorMessage = "TextRequired")]
    [StringLength(10000, ErrorMessage = "TextMaxLength")]
    public string Text { get; set; } = string.Empty;
    
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
}