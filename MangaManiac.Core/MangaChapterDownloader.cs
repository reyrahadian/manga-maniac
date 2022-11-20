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
            var httpClients = new List<HttpClient>();
            var downloadImageTasks = new List<Task<HttpResponseMessage>>();
            foreach (var chapterImage in chapterImages)
            {
                var httpClient = new HttpClient();
                downloadImageTasks.Add(httpClient.GetAsync(chapterImage.ImageUri));
                httpClients.Add(httpClient);
            }
            await Task.WhenAll(downloadImageTasks);
            httpClients.ForEach(h => h.Dispose());

            var saveFileTasks = new List<Task>();
            var fileStreams = new List<FileStream>();
            int fileCounter = 1;
            foreach (var downloadImageTask in downloadImageTasks)
            {
                var chapterImage = chapterImages.ElementAt(fileCounter-1);
                var originalFileName = chapterImage.ImageUri.Segments.Last();
                var filePath = $"{chapterDirPath}\\{fileCounter}-{originalFileName}";
                var stream = new FileStream(filePath, FileMode.Create);
                saveFileTasks.Add(downloadImageTask.Result.Content.CopyToAsync(stream));
                fileStreams.Add(stream);

                fileCounter++;
            }
            await Task.WhenAll(saveFileTasks);
            fileStreams.ForEach(s => s.Dispose());
            _logger.Information($"All chapter images have been downloaded to {chapterDirPath}");
        }
    }
}
