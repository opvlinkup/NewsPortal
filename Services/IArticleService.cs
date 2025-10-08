using NewsPortal.Models;

public interface IArticleService
{
    Task<Article?> GetByIdAsync(Guid id);
    Task<List<Article>> GetPagedAsync(int skip, int take);
    Task<List<Article>> GetLatestAsync(int count);
    Task<Article> CreateAsync(Article article, IFormFile? imageFile);
    Task UpdateAsync(Article article, IFormFile? imageFile);
    Task DeleteAsync(Guid id);
}
