using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlShortenerController(UrlShortenerContext context) : ControllerBase
{
    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl(string originalUrl)
    {
        var shortenedUrl = Guid.NewGuid().ToString("B").Substring(0, 8);
        var mapping = new UrlMapping
        {
            OriginalUrl = originalUrl,
            ShortenedUrl = shortenedUrl
        };

        context.UrlMappings.Add(mapping);
        await context.SaveChangesAsync();

        return Ok(new { shortenedUrl });
    }

    [HttpGet("getOriginal/{shortenedUrl}")]
    public async Task<IActionResult> GetOriginalUrl(string shortenedUrl)
    {
        var mapping = await context.UrlMappings
            .FirstOrDefaultAsync(m => m.ShortenedUrl == shortenedUrl);

        if (mapping == null)
            return NotFound("URL not found");

        // Return Original URL
        return Ok(new { originalUrl = mapping.OriginalUrl });
    }
    
    [HttpGet("redirectToOriginal/{shortenedUrl}")]
    public async Task<IActionResult> RedirectToUrl(string shortenedUrl)
    {
        var mapping = await context.UrlMappings
            .FirstOrDefaultAsync(m => m.ShortenedUrl == shortenedUrl);

        if (mapping == null)
            return NotFound("URL not found");

        // Redirect to original URL using shorten URl
        return Redirect(mapping.OriginalUrl);
    }
}