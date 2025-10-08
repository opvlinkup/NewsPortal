using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ArticleTranslation> Translations { get; set; } =
            new List<ArticleTranslation>();
    }
}
