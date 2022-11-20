namespace MangaManiac.Core.Models
{
    public class Manga
    {
        public string Title { get; set; }
        public IEnumerable<MangaChapter> Chapters { get; set; }
    }
}
