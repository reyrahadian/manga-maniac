using MangaManiac.Core;
using MangaManiac.Core.HtmlPageParsers;
using MangaManiac.Core.Models;
using Newtonsoft.Json;
using Serilog;

namespace MangaManiac.Updater.Console
{
    internal class MangaUpdater
    {
        private ILogger _logger;
        private MangaInfoPageParser _mangaInfoPageParser;

        public MangaUpdater(ILogger logger)
        {
            _logger = logger;
            _mangaInfoPageParser = new MangaInfoPageParser(logger);
        }

        public async Task UpdateAsync(string mangaDirPath)
        {
            try
            {
                // load artifact file
                var artifactFilePath = $"{mangaDirPath}\\manga.json";
                _logger.Information($"Loading artifact definition from {artifactFilePath}");
                var artifactContent = await File.ReadAllTextAsync(artifactFilePath);
                var artifact = JsonConvert.DeserializeObject<MangaArtifact>(artifactContent);

                // get chapters from the website
                var mangaInfo = await _mangaInfoPageParser.GetMangaInfoAsync(artifact.Uri);
                var newChapterNumbers = mangaInfo.Chapters.Select(c => c.Number.GetValueOrDefault()).Except(artifact.Chapters.Select(ac => ac.Number));
                var newChapters = mangaInfo.Chapters.Where(c => newChapterNumbers.Contains(c.Number.GetValueOrDefault()));

                _logger.Information($"{newChapters.Count()} new chapters found");
                await UpdateInBatchAsync(mangaDirPath, newChapters);
                foreach (var chapter in newChapters)
                {
                    artifact.Chapters.Add(new MangaArtifact.Chapter
                    {
                        Number = chapter.Number.GetValueOrDefault(),
                        Title = chapter.Title,
                        Uri = chapter?.Uri
                    });
                }

                // update artifact file
                artifact.LastModified = DateTime.UtcNow;
                artifact.Chapters = artifact.Chapters.OrderByDescending(c => c.Number).ToList();
                await File.WriteAllTextAsync(artifactFilePath, JsonConvert.SerializeObject(artifact));
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error");
                throw;
            }
        }

        private async Task UpdateInBatchAsync(string mangaDirPath, IEnumerable<MangaChapter> newChapters, int currentPageIndex = 0)
        {
            var numOfChaptersInABatch = 10;
            var chaptersToProcess = newChapters.Skip(currentPageIndex * numOfChaptersInABatch).Take(numOfChaptersInABatch);
            var totalPages = Math.Ceiling((decimal)newChapters.Count() / numOfChaptersInABatch);

            var tasks = new List<Task>();
            foreach (var chapter in chaptersToProcess)
            {
                var chapterDirPath = $"{mangaDirPath}\\{chapter.Title}";
                tasks.Add(new MangaChapterDownloader(_logger).DownloadAsync(chapterDirPath, chapter));
            }
            await Task.WhenAll(tasks);

            if (currentPageIndex < totalPages)
            {
                currentPageIndex++;
                await UpdateInBatchAsync(mangaDirPath, newChapters, currentPageIndex);
            }
        }
    }
}
