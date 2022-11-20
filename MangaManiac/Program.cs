using MangaManiac.Updater.Console;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
     .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var mangaRootDirPath = @"C:\Users\reyrahadian\Downloads\manga-m";
var mangaDirectories = Directory.GetDirectories(mangaRootDirPath).Select(d => new DirectoryInfo(d));
logger.Information($"Scanning manga directories");
foreach (var directory in mangaDirectories)
{
    logger.Information($"Found {directory.FullName}");
    await new MangaUpdater(logger).UpdateAsync(directory.FullName);    
}
logger.Information($"All mangas are up to date");

Console.ReadLine();