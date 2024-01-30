namespace Chameleon.Interfaces.Prospector
{
    public interface IGoogleCrawlerResponse
    {
        string Response { get; set; }
    }

    public interface IGoogleCrawlerSearchResult
    {
        string Keyword { get; set; }
        string DomainScore { get; set; }
        string Link { get; set; }
        string SearchEngine { get; set; }
        int Position { get; set; }
        int TimesKwFound { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Published { get; set; }
        string Lang { get; set; }
        string Pnum { get; set; }
        string Ptotal { get; set; }
        string PerformanceScore { get; set; }
        string Country { get; set; }
        string Type { get; set; }
    }
}
