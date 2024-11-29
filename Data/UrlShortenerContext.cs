using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Data;

public class UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : DbContext(options)
{
    public DbSet<UrlMapping> UrlMappings { get; set; }
}