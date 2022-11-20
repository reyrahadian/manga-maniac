using MangaManiac.Core.HtmlPageParsers;
using MangaManiac.Core.Models;
using Newtonsoft.Json;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
     .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var mangaRootDirPath = @"C:\Users\reyrahadian\Downloads\manga-m";
var newMangaUrlToAdd = "https://asura.gg/manga/chronicles-of-the-martial-gods-return/";

var manga = await new MangaInfoPageParser(logger).GetMangaInfoAsync(new Uri(newMangaUrlToAdd));
var mangaDirPath = $"{mangaRootDirPath}\\{manga.Title}";
if (!Directory.Exists(mangaDirPath))
{
    Directory.CreateDirectory(mangaDirPath);
    var artifact = new MangaArtifact
    {
        Title = manga.Title,
        Uri = new Uri(newMangaUrlToAdd),
        LastModified = DateTime.UtcNow
    };
    var artifactFilePath = $"{mangaDirPath}\\manga.json";
    await File.WriteAllTextAsync(artifactFilePath, JsonConvert.SerializeObject(artifact));
}