using HtmlAgilityPack;
using MangaManiac.Core.Models;
using Serilog;
using System.Globalization;

namespace MangaManiac.Core.HtmlPageParsers
{
    public class MangaInfoPageParser
    {
        private readonly ILogger _logger;

        public MangaInfoPageParser(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<Manga> GetMangaInfoAsync(Uri mangaInfoPageUrl)
        {
            _logger.Information($"Retrieving manga information from {mangaInfoPageUrl}");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(mangaInfoPageUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var manga = new Manga();
                var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[@class='entry-title']");
                manga.Title = titleNode.InnerHtml;

                var chapterNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='chapterlist']/ul/*");
                var chapters = new List<MangaChapter>();
                foreach (var chapterNode in chapterNodes)
                {
                    var chapter = new MangaChapter();
                    float chapterNumber;
                    if (float.TryParse(chapterNode.Attributes["data-num"].Value, out chapterNumber))
                    {
                        chapter.Number = chapterNumber;
                    }

                    var chapterTitleNode = chapterNode.Descendants()
                        .Where(n => n.Name == "span" && n.Attributes["class"].Value=="chapternum")
                        .FirstOrDefault();
                    if (chapterTitleNode != null)
                    {
                        chapter.Title = chapterTitleNode.InnerHtml;
                    }

                    var chapterDateNode = chapterNode.Descendants()
                        .Where(n => n.Name == "span" && n.Attributes["class"].Value=="chapterdate")
                        .FirstOrDefault();
                    if (chapterDateNode != null)
                    {
                        if (DateTime.TryParseExact(chapterDateNode.InnerHtml, "MMMM dd, yyyy", new CultureInfo("en-us"), DateTimeStyles.None, out var chapterDate))
                        {
                            chapter.Date = chapterDate;
                        }
                    }

                    var test = chapterNode.Descendants();
                    var urlNode = chapterNode.Descendants()
                        .Where(n => n.Name == "a")
                        .FirstOrDefault();
                    if (urlNode != null)
                    {
                        chapter.Uri = new Uri(urlNode.Attributes["href"].Value);
                    }

                    chapters.Add(chapter);
                }

                manga.Chapters = chapters.OrderByDescending(c => c.Number);

                return manga;
            }
        }
    }
}
