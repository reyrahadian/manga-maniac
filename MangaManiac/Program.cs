using MangaManiac.Console.HtmlPageParsers;

var manga = await new ChapterLandingPageParser().GetMangaInfoAsync(new Uri("https://asura.gg/manga/chronicles-of-the-martial-gods-return/"));
var mangaRootDirPath = @"C:\Users\reyrahadian\Downloads\manga-m";

var mangaDirPath = $"{mangaRootDirPath}\\{manga.Title}";
if (!Directory.Exists(mangaDirPath))
{
    Directory.CreateDirectory(mangaDirPath);
}

foreach (var chapter in manga.Chapters.Take(1))
{
    var chapterDirPath = $"{mangaDirPath}\\{chapter.Title}";
    if (!Directory.Exists(chapterDirPath))
    {
        Directory.CreateDirectory(chapterDirPath);
    }

    var chapterImages = await new ChapterDetailPageParser().GetChapterImagesAsync(chapter.Uri);
    var httpClients = new List<HttpClient>();
    var downloadImageTasks = new List<Task<HttpResponseMessage>>();
    foreach (var chapterImage in chapterImages)
    {
        var httpClient = new HttpClient();
        downloadImageTasks.Add(httpClient.GetAsync(chapterImage.ImageUri));
        httpClients.Add(httpClient);
    }
    await Task.WhenAll(downloadImageTasks);
    httpClients.ForEach(h => h.Dispose());

    var saveFileTasks = new List<Task>();
    var fileStreams = new List<FileStream>();
    int fileCounter = 1;
    foreach (var downloadImageTask in downloadImageTasks)
    {
        var chapterImage = chapterImages.ElementAt(fileCounter-1);
        var fileExt = chapterImage.ImageUri.Segments.Last().Substring(chapterImage.ImageUri.Segments.Last().IndexOf("."));
        var filePath = $"{chapterDirPath}\\{fileCounter}{fileExt}";
        var stream = new FileStream(filePath, FileMode.Create);
        saveFileTasks.Add(downloadImageTask.Result.Content.CopyToAsync(stream));
        fileStreams.Add(stream);

        fileCounter++;
    }
    await Task.WhenAll(saveFileTasks);
    fileStreams.ForEach(s => s.Dispose());
}

Console.ReadLine();