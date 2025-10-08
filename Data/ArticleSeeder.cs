using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Models;

namespace NewsPortal.Seeds
{
    public static class ArticleSeeder
    {
        public static async Task SeedAsync(NPDbContext context)
        {
            if (await context.Articles.AnyAsync())
                return;

            var articles = new List<Article>();
            for (var i = 1; i <= 10; i++)
            {
                var articleId = Guid.NewGuid();
                articles.Add(new Article
                {
                    Id = articleId,
                    ImagePath = $"/uploads/{i}.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    Translations = new List<ArticleTranslation>
                    {
                        new ArticleTranslation
                        {
                            Id = Guid.NewGuid(),
                            Language = "ru",
                            Title = $"Заголовок статьи {i}",
                            Subtitle = $"Подзаголовок статьи {i}",
                            Text = $"Это текст статьи номер {i}. Здесь рассказывается о важной теме для читателей."
                        },
                        new ArticleTranslation
                        {
                            Id = Guid.NewGuid(),
                            Language = "en",
                            Title = $"Article Title {i}",
                            Subtitle = $"Article Subtitle {i}",
                            Text = $"This is the text of article number {i}. It discusses an important topic for readers."
                        }
                    }
                });
            }

            await context.Articles.AddRangeAsync(articles);
            await context.SaveChangesAsync();
        }
    }
}