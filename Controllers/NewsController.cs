using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Models.ViewModels;
using NewsPortal.Services;

namespace NewsPortal.Controllers
{
    public class NewsController(ILogger<NewsController> logger, IArticleService articleService)
        : Controller
    {
        private string GetCurrentLanguage()
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            logger.LogDebug("Current UI culture detected: {Culture}", culture);
            return culture;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var culture = GetCurrentLanguage();
            logger.LogInformation("Fetching latest articles for culture {Culture}", culture);

            var articles = await articleService.GetLatestAsync(6, cancellationToken);
            logger.LogDebug("Fetched {Count} latest articles", articles.Count);

            var articleVms = articles
                .Select(a =>
                {
                    var tr =
                        a.Translations.FirstOrDefault(t => t.Language == culture)
                        ?? a.Translations.FirstOrDefault(t => t.Language == "ru");

                    if (tr == null)
                    {
                        logger.LogWarning(
                            "No translation found for article {ArticleId}, fallback to empty",
                            a.Id
                        );
                    }

                    string? imageUrl = null;
                    if (!string.IsNullOrWhiteSpace(a.ImagePath))
                    {
                        var path = a.ImagePath.Replace('\\', '/').Trim();
                        path = path.TrimStart('~').TrimStart('/');
                        imageUrl = string.Join("/", path);
                    }

                    return new ArticleViewModel
                    {
                        Id = a.Id,
                        Title = tr?.Title ?? "No title",
                        Subtitle = tr?.Subtitle,
                        Text = tr?.Text ?? "",
                        CreatedAt = a.CreatedAt,
                        ImagePath = imageUrl
                    };
                })
                .ToList();

            return View(articleVms);
        }

        [HttpGet("All")]
        public IActionResult All()
        {
            logger.LogInformation("Rendering All view");
            return View();
        }

        [HttpGet("GetArticles")]
        public async Task<IActionResult> GetArticles(int skip = 0, int take = 6, CancellationToken cancellationToken = default)
        {
            var culture = GetCurrentLanguage();
            logger.LogInformation(
                "Fetching articles batch: skip={Skip}, take={Take}, culture={Culture}",
                skip,
                take,
                culture
            );

            var articles = await articleService.GetPagedAsync(skip, take, cancellationToken);

            if (!articles.Any())
            {
                logger.LogInformation(
                    "No more articles found for skip={Skip}, take={Take}",
                    skip,
                    take
                );
            }

            var vms = articles
                .Select(a =>
                {
                    var tr =
                        a.Translations.FirstOrDefault(t => t.Language == culture)
                        ?? a.Translations.FirstOrDefault(t => t.Language == "ru");

                    return new
                    {
                        id = a.Id,
                        title = tr?.Title ?? "No title",
                        subtitle = tr?.Subtitle,
                        text = tr?.Text ?? "",
                        createdAt = a.CreatedAt,
                        imagePath = string.IsNullOrWhiteSpace(a.ImagePath)
                            ? null
                            : a.ImagePath.Replace("\\", "/")
                    };
                })
                .ToList();

            return Json(vms);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
        {
            var culture = GetCurrentLanguage();
            logger.LogInformation(
                "Fetching article details for {ArticleId}, culture={Culture}",
                id,
                culture
            );

            var article = await articleService.GetByIdAsync(id, cancellationToken);
            if (article == null)
            {
                logger.LogWarning("Article {ArticleId} not found", id);
                return NotFound();
            }

            var tr =
                article.Translations.FirstOrDefault(t => t.Language == culture)
                ?? article.Translations.FirstOrDefault(t => t.Language == "ru");

            if (tr == null)
            {
                logger.LogWarning("No translation found for article {ArticleId}", id);
            }

            var viewModel = new ArticleViewModel
            {
                Id = article.Id,
                Title = tr?.Title ?? "No title",
                Subtitle = tr?.Subtitle,
                Text = tr?.Text ?? "",
                CreatedAt = article.CreatedAt,
                ImagePath = article.ImagePath
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                logger.LogInformation("Setting UI culture to {Culture}", culture);

                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }
            else
            {
                logger.LogWarning("Attempted to set empty culture");
            }

            return LocalRedirect(returnUrl ?? Url.Action("Index", "News")!);
        }
    }
}
