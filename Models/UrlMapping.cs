namespace UrlShortener.Models;

public class UrlMapping
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = null!;
    public string ShortenedUrl { get; set; } = null!;
}