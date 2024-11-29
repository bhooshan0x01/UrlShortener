using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Repositories;

public class UrlRepository : IUrlRepository
{
    private readonly UrlShortenerContext _context;
    private readonly ILogger<UrlRepository> _logger;

    public UrlRepository(UrlShortenerContext context, ILogger<UrlRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UrlMapping?> GetByOriginalUrlAsync(string originalUrl)
    {
        try
        {
            return await _context.UrlMappings
                .FirstOrDefaultAsync(m => m.OriginalUrl == originalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving URL mapping for original URL: {OriginalUrl}", originalUrl);
            throw;
        }
    }

    public async Task<UrlMapping?> GetByShortenedUrlAsync(string shortenedUrl)
    {
        try
        {
            return await _context.UrlMappings
                .FirstOrDefaultAsync(m => m.ShortenedUrl == shortenedUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving URL mapping for shortened URL: {ShortenedUrl}", shortenedUrl);
            throw;
        }
    }

    public async Task<UrlMapping> CreateAsync(string originalUrl)
    {
        try
        {
            var shortenedUrl = Guid.NewGuid().ToString("N").Substring(0, 8);
            var mapping = new UrlMapping
            {
                OriginalUrl = originalUrl,
                ShortenedUrl = shortenedUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UrlMappings.AddAsync(mapping);
            return mapping;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating URL mapping for: {OriginalUrl}", originalUrl);
            throw;
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }
}