using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Models.ViewModels
{
        public class ArticleEditViewModel
        {
            public Guid? Id { get; set; }

            //Common
            public IFormFile? ImageFile { get; set; }
            public string? ExistingImagePath { get; set; }

            // RU
            public string TitleRu { get; set; } = string.Empty;
            public string? SubtitleRu { get; set; }
            public string TextRu { get; set; } = string.Empty;

            // EN
            public string TitleEn { get; set; } = string.Empty;
            public string? SubtitleEn { get; set; }
            public string TextEn { get; set; } = string.Empty;
        }
}
