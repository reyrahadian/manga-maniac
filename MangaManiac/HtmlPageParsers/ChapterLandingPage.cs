using HtmlAgilityPack;
using MangaManiac.Console.Models;
using System.Globalization;

namespace MangaManiac.Console.HtmlPageParsers
{
    internal class ChapterLandingPageParser
    {
        public async Task<IEnumerable<Chapter>> GetChaptersAsync(Uri chapterlandingPageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(chapterlandingPageUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var chapterNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='chapterlist']/ul/*");
                var chapters = new List<Chapter>();
                foreach (var chapterNode in chapterNodes)
                {
                    var chapter = new Chapter();
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
                        if (DateTime.TryParseExact(chapterDateNode.InnerHtml, "MMMM dd, yyyy", new CultureInfo("en-us"), System.Globalization.DateTimeStyles.None, out var chapterDate))
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

                return chapters.OrderByDescending(c => c.Number);
            }
        }
    }
}
