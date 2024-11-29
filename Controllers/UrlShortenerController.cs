using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Repositories;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlRepository _repository;
    private readonly ILogger<UrlShortenerController> _logger;

    public UrlShortenerController(IUrlRepository repository, ILogger<UrlShortenerController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("shorten")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ShortenUrl([Required] string originalUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                return BadRequest("URL cannot be empty");
            }

            if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uriResult) 
                || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest("Invalid URL format. URL must start with http:// or https://");
            }

            // Check if URL already exists
            var existingMapping = await _repository.GetByOriginalUrlAsync(originalUrl);
            if (existingMapping != null)
            {
                return Ok(new { shortenedUrl = existingMapping.ShortenedUrl, message = "URL was already shortened" });
            }

            var mapping = await _repository.CreateAsync(originalUrl);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("URL shortened successfully: {OriginalUrl} -> {ShortenedUrl}", 
                originalUrl, mapping.ShortenedUrl);
            return Ok(new { shortenedUrl = mapping.ShortenedUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shortening URL: {OriginalUrl}", originalUrl);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("expand/{shortenedUrl}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExpandUrl([Required] string shortenedUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(shortenedUrl))
            {
                return BadRequest("Shortened URL cannot be empty");
            }

            if (shortenedUrl.Length != 8)
            {
                return BadRequest("Invalid shortened URL format");
            }

            var mapping = await _repository.GetByShortenedUrlAsync(shortenedUrl);
            if (mapping == null)
            {
                _logger.LogWarning("Shortened URL not found: {ShortenedUrl}", shortenedUrl);
                return NotFound("URL not found");
            }

            _logger.LogInformation("URL expanded successfully: {ShortenedUrl} -> {OriginalUrl}", 
                shortenedUrl, mapping.OriginalUrl);
            return Ok(new { originalUrl = mapping.OriginalUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error expanding URL: {ShortenedUrl}", shortenedUrl);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("redirect/{shortenedUrl}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RedirectToUrl([Required] string shortenedUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(shortenedUrl))
            {
                return BadRequest("Shortened URL cannot be empty");
            }

            if (shortenedUrl.Length != 8)
            {
                return BadRequest("Invalid shortened URL format");
            }

            var mapping = await _repository.GetByShortenedUrlAsync(shortenedUrl);
            if (mapping == null)
            {
                _logger.LogWarning("Shortened URL not found for redirect: {ShortenedUrl}", shortenedUrl);
                return NotFound("URL not found");
            }

            _logger.LogInformation("Redirecting: {ShortenedUrl} -> {OriginalUrl}", 
                shortenedUrl, mapping.OriginalUrl);
            return Redirect(mapping.OriginalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting URL: {ShortenedUrl}", shortenedUrl);
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}