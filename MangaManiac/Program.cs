// See https://aka.ms/new-console-template for more information
using MangaManiac.Console.HtmlPageParsers;

var chapters = await new ChapterLandingPageParser().GetChaptersAsync(new Uri("https://asura.gg/manga/chronicles-of-the-martial-gods-return/"));
var chapterDetail = await new ChapterDetailPageParser().GetChapterImagesAsync(chapters.First().Uri);
Console.ReadLine();