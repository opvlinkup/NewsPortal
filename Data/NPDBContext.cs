using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Models;

namespace NewsPortal.Data;

public class NPDbContext : IdentityDbContext<IdentityUser>
{
    public NPDbContext(DbContextOptions<NPDbContext> options) : base(options) {}
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleTranslation> ArticleTranslations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasIndex(a => a.CreatedAt);

        modelBuilder.Entity<ArticleTranslation>()
            .HasIndex(t => new { t.ArticleId, t.Language });
    }

}
