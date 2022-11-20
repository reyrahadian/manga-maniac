namespace MangaManiac.Console.Models
{
    internal class Manga
    {
        public string Title { get; set; }
        public IEnumerable<Chapter> Chapters { get; set; }
    }
}
