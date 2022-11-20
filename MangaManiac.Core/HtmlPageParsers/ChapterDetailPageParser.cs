using HtmlAgilityPack;
using MangaManiac.Console.Models;
using Serilog;

namespace MangaManiac.Core.HtmlPageParsers
{
    public class MangaChapterDetailPageParser
    {
        private ILogger _logger;

        public MangaChapterDetailPageParser(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<MangaChapterImage>> GetChapterImagesAsync(Uri chapterDetailPageUrl)
        {
            _logger.Information($"Retrieving images from {chapterDetailPageUrl}");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(chapterDetailPageUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var readerAreaNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='readerarea']");
                var imageNodes = readerAreaNode.Descendants().Where(n => n.Name == "img");

                var chapterImages = new List<MangaChapterImage>();
                int order = 1;
                foreach (var imageNode in imageNodes)
                {
                    var chapterImage = new MangaChapterImage();

                    var imageSrc = imageNode.GetAttributeValue("src", string.Empty);
                    if (string.IsNullOrEmpty(imageSrc))
                    {
                        continue;
                    }

                    chapterImage.ImageUri = new Uri(imageSrc);
                    chapterImage.Order = order;
                    chapterImages.Add(chapterImage);

                    order++;
                }

                return chapterImages.OrderBy(c => c.Order);
            }
        }
    }
}