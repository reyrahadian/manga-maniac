using System.IO;

namespace MangaManiac.Core
{
    internal class BulkImageDownloader
    {
        public async Task<IEnumerable<string>> DownloadAsync(string imageDirPath, IEnumerable<Uri> uris)
        {
            int fileCounter = 1;
            var filePaths = new List<string>();
            foreach (var uri in uris)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(uri);
                    var originalFileName = uri.Segments.Last();
                    var filePath = $"{imageDirPath}\\{fileCounter}-{originalFileName}";
                    var stream = new FileStream(filePath, FileMode.Create);
                    await response.Content.CopyToAsync(stream);
                    filePaths.Add(filePath);
                }
                fileCounter++;
            }

            return filePaths;
        }
    }
}
