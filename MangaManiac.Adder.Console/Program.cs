using MangaManiac.Adder.Console;
using MangaManiac.Core.HtmlPageParsers;
using MangaManiac.Core.Models;
using Newtonsoft.Json;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
     .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
var newMangaUrlToAdd = "https://asura.gg/manga/chronicles-of-the-martial-gods-return/";

logger.Information($"Adding new manga from {newMangaUrlToAdd}");
var manga = await new MangaInfoPageParser(logger).GetMangaInfoAsync(new Uri(newMangaUrlToAdd));
var mangaDirPath = $"{config.MangaRootDirPath}\\{manga.Title}";
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
else {
    logger.Information($"\"{mangaDirPath}\" already exist");
}