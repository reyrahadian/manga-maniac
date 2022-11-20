using MangaManiac.Core.HtmlPageParsers;
using MangaManiac.Core.Models;
using Serilog;

namespace MangaManiac.Core
{
    public class MangaChapterDownloader
    {
        private ILogger _logger;
        private MangaChapterDetailPageParser _mangaChapterDetailPageParser;

        public MangaChapterDownloader(ILogger logger)
        {
            _logger = logger;
            _mangaChapterDetailPageParser = new MangaChapterDetailPageParser(logger);
        }

        public async Task DownloadAsync(string chapterDirPath, MangaChapter mangaChapter)
        {
            _logger.Information($"Downloading chapter images to {chapterDirPath}");
            if (!Directory.Exists(chapterDirPath))
            {
                Directory.CreateDirectory(chapterDirPath);
            }

            var chapterImages = await _mangaChapterDetailPageParser.GetChapterImagesAsync(mangaChapter.Uri);
            var filePaths = await new BulkImageDownloader().DownloadAsync(chapterDirPath, chapterImages.Select(img => img.ImageUri));
            foreach (var filePath in filePaths)
            {
                var pdfFilePath = $"{filePath}.pdf";
                new ImageToPdfConverter().Convert(pdfFilePath, filePath);
                File.Delete(filePath);
            }            

            _logger.Information($"All chapter images have been downloaded to {chapterDirPath}");
        }
    }
}
