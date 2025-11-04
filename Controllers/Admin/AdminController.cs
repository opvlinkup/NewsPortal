using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Models;
using NewsPortal.Models.ViewModels;
using NewsPortal.Services;

namespace NewsPortal.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Articles")]
    public class AdminController : Controller
    {
        private readonly IArticleService _service;

        public AdminController(IArticleService service) => _service = service;

        [HttpGet("")]
        public async Task<IActionResult> Index() => View(await _service.GetPagedAsync(0, 100));

        [HttpGet("Create")]
        public IActionResult Create() => View(new ArticleEditViewModel());

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var article = new Article();

            article.Translations.Add(
                new ArticleTranslation
                {
                    Language = "ru",
                    Title = vm.TitleRu,
                    Subtitle = vm.SubtitleRu,
                    Text = vm.TextRu
                }
            );

            article.Translations.Add(
                new ArticleTranslation
                {
                    Language = "en",
                    Title = vm.TitleEn,
                    Subtitle = vm.SubtitleEn,
                    Text = vm.TextEn
                }
            );

            await _service.CreateAsync(article, vm.ImageFile);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _service.GetByIdAsync(id);
            if (article == null)
                return NotFound();

            var vm = new ArticleEditViewModel
            {
                Id = article.Id,
                ExistingImagePath = article.ImagePath,
                TitleRu = article.Translations.FirstOrDefault(t => t.Language == "ru")?.Title ?? "",
                SubtitleRu = article.Translations.FirstOrDefault(t => t.Language == "ru")?.Subtitle,
                TextRu = article.Translations.FirstOrDefault(t => t.Language == "ru")?.Text ?? "",
                TitleEn = article.Translations.FirstOrDefault(t => t.Language == "en")?.Title ?? "",
                SubtitleEn = article.Translations.FirstOrDefault(t => t.Language == "en")?.Subtitle,
                TextEn = article.Translations.FirstOrDefault(t => t.Language == "en")?.Text ?? ""
            };

            return View(vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ArticleEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var article = await _service.GetByIdAsync(id);
            if (article == null)
                return NotFound();

            UpdateTranslation(article, "ru", vm.TitleRu, vm.SubtitleRu, vm.TextRu);
            UpdateTranslation(article, "en", vm.TitleEn, vm.SubtitleEn, vm.TextEn);

            await _service.UpdateAsync(article, vm.ImageFile);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _service.GetByIdAsync(id);
            return article == null ? NotFound() : View(article);
        }

        [HttpPost("Delete/{id:guid}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void UpdateTranslation(
            Article article,
            string lang,
            string title,
            string? subtitle,
            string text
        )
        {
            var tr = article.Translations.FirstOrDefault(t => t.Language == lang);
            if (tr != null)
            {
                tr.Title = title;
                tr.Subtitle = subtitle;
                tr.Text = text;
            }
            else
            {
                article.Translations.Add(
                    new ArticleTranslation
                    {
                        Language = lang,
                        Title = title,
                        Subtitle = subtitle,
                        Text = text
                    }
                );
            }
        }
    }
}
