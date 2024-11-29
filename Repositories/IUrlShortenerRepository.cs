using UrlShortener.Models;

namespace UrlShortener.Repositories;

public interface IUrlRepository
{
    Task<UrlMapping?> GetByOriginalUrlAsync(string originalUrl);
    Task<UrlMapping?> GetByShortenedUrlAsync(string shortenedUrl);
    Task<UrlMapping> CreateAsync(string originalUrl);
    Task<bool> SaveChangesAsync();
}