using MangaManiac.Updater.Console;
using Newtonsoft.Json;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
     .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
var mangaDirectories = Directory.GetDirectories(config.MangaRootDirPath).Select(d => new DirectoryInfo(d));
logger.Information($"Scanning manga directories");
foreach (var directory in mangaDirectories)
{
    logger.Information($"Found {directory.FullName}");
    await new MangaUpdater(logger).UpdateAsync(directory.FullName);    
}
logger.Information($"All mangas are up to date");