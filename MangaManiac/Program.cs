// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using MangaManiac.Console.HtmlPageParsers;
using MangaManiac.Console.Models;
using System.Globalization;

var chapters = await new ChapterLandingPageParser().GetChaptersAsync(new Uri("https://asura.gg/manga/chronicles-of-the-martial-gods-return/"));
Console.ReadLine();