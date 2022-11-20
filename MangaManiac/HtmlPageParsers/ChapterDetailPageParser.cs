using HtmlAgilityPack;
using MangaManiac.Console.Models;

namespace MangaManiac.Console.HtmlPageParsers
{
    internal class ChapterDetailPageParser
    {
        public async Task<IEnumerable<ChapterImage>> GetChapterImagesAsync(Uri chapterDetailPageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(chapterDetailPageUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var readerAreaNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='readerarea']");
                var imageNodes = readerAreaNode.Descendants().Where(n => n.Name == "img");

                var chapterImages = new List<ChapterImage>();
                int order = 1;
                foreach (var imageNode in imageNodes)
                {
                    var chapterImage = new ChapterImage();

                    var cfImageSrcAttribute = imageNode.GetDataAttribute("cfsrc");
                    var imageSrc = imageNode.GetAttributeValue("src", cfImageSrcAttribute?.Value);
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