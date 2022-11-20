namespace MangaManiac.Core
{
    internal class BulkImageDownloader
    {
        public async Task<IEnumerable<string>> DownloadAsync(string imageDirPath, IEnumerable<Uri> uris)
        {
            var httpClients = new List<HttpClient>();
            var downloadImageTasks = new List<Task<HttpResponseMessage>>();
            foreach (var uri in uris)
            {
                var httpClient = new HttpClient();
                downloadImageTasks.Add(httpClient.GetAsync(uri));
                httpClients.Add(httpClient);
            }
            await Task.WhenAll(downloadImageTasks);
            httpClients.ForEach(h => h.Dispose());

            var saveFileTasks = new List<Task>();
            var fileStreams = new List<FileStream>();
            var filePaths = new List<string>();
            int fileCounter = 1;
            foreach (var downloadImageTask in downloadImageTasks)
            {
                var uri = uris.ElementAt(fileCounter-1);
                var originalFileName = uri.Segments.Last();
                var filePath = $"{imageDirPath}\\{fileCounter}-{originalFileName}";
                var stream = new FileStream(filePath, FileMode.Create);
                saveFileTasks.Add(downloadImageTask.Result.Content.CopyToAsync(stream));
                fileStreams.Add(stream);
                filePaths.Add(filePath);

                fileCounter++;
            }
            await Task.WhenAll(saveFileTasks);
            fileStreams.ForEach(s => s.Dispose());

            return filePaths;
        }
    }
}
