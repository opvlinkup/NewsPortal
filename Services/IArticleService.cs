using NewsPortal.Models;

namespace NewsPortal.Services;

public interface IArticleService
{
    Task<Article?> GetByIdAsync(Guid id, CancellationToken ctx = default);
    Task<List<Article>> GetPagedAsync(int skip, int take, CancellationToken ctx = default);
    Task<List<Article>> GetLatestAsync(int count, CancellationToken ctx = default);
    Task<Article> CreateAsync(Article article, IFormFile? imageFile, CancellationToken ctx = default);
    Task UpdateAsync(Article article, IFormFile? imageFile, CancellationToken ctx = default);
    Task DeleteAsync(Guid id, CancellationToken ctx = default);
}