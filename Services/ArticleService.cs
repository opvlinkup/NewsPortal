using Microsoft.EntityFrameworkCore;
using NewsPortal.Data;
using NewsPortal.Models;

namespace NewsPortal.Services
{
    public class ArticleService : IArticleService
    {
        private readonly NPDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticleService(NPDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<Article>> GetPagedAsync(
            int skip,
            int take,
            CancellationToken ctx = default
        ) =>
            await _context
                .Articles.OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Include(a => a.Translations)
                .ToListAsync(ctx);

        public async Task<Article?> GetByIdAsync(Guid id, CancellationToken ctx = default)
        {
            return await _context
                .Articles.Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.Id == id, ctx);
        }

        public async Task<List<Article>> GetLatestAsync(int count, CancellationToken ctx = default)
        {
            if (count <= 0)
                return new List<Article>();

            return await _context
                .Articles.Include(a => a.Translations)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync(ctx);
        }

        public async Task<Article> CreateAsync(
            Article article,
            IFormFile? imageFile,
            CancellationToken ctx = default
        )
        {
            if (imageFile is not null)
            {
                article.ImagePath = await SaveImageAsync(imageFile, ctx);
            }

            await _context.Articles.AddAsync(article, ctx);
            await _context.SaveChangesAsync(ctx);
            return article;
        }

        public async Task UpdateAsync(
            Article article,
            IFormFile? imageFile,
            CancellationToken ctx = default
        )
        {
            var existing =
                await _context
                    .Articles.Include(a => a.Translations)
                    .FirstOrDefaultAsync(a => a.Id == article.Id, ctx)
                ?? throw new KeyNotFoundException("Статья не найдена");

            existing.UpdatedAt = DateTime.UtcNow;

            if (imageFile is not null)
            {
                DeleteImageIfExists(existing.ImagePath);
                existing.ImagePath = await SaveImageAsync(imageFile, ctx);
            }

            foreach (var translation in article.Translations)
            {
                var existingTranslation = existing.Translations.FirstOrDefault(t =>
                    t.Language == translation.Language
                );

                if (existingTranslation is not null)
                {
                    existingTranslation.Title = translation.Title;
                    existingTranslation.Subtitle = translation.Subtitle;
                    existingTranslation.Text = translation.Text;
                }
                else
                {
                    existing.Translations.Add(
                        new ArticleTranslation
                        {
                            Language = translation.Language,
                            Title = translation.Title,
                            Subtitle = translation.Subtitle,
                            Text = translation.Text
                        }
                    );
                }
            }

            await _context.SaveChangesAsync(ctx);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ctx = default)
        {
            var article =
                await _context
                    .Articles.Include(a => a.Translations)
                    .FirstOrDefaultAsync(a => a.Id == id, ctx)
                ?? throw new KeyNotFoundException("Статья не найдена");

            DeleteImageIfExists(article.ImagePath);

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync(ctx);
        }

        // -------- utils --------

        private async Task<string> SaveImageAsync(IFormFile file, CancellationToken ctx = default)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, ctx);

            return "/uploads/" + fileName;
        }

        private void DeleteImageIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
