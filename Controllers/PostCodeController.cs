using Microsoft.AspNetCore.Mvc;
using PostCode.Helpers;
using System.Text.RegularExpressions;
using System.Web;
using PostCode.Dto;
using PostCode.Config;
using Microsoft.Extensions.Options;

namespace PostCode.Controllers;

[ApiController]
[Route("[controller]")]
public class PostCodeController : ControllerBase
{
    private static Regex _regex = new Regex(@"\s+|\-", RegexOptions.Compiled);

    private readonly ILogger<PostCodeController> _logger;
    private readonly PostCodeAnywhereConfig _config;

    public PostCodeController(ILogger<PostCodeController> logger,
        IOptions<PostCodeAnywhereConfig> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    private async Task<SearchResultItemDto[]> GetSearchResults(string text)
    {
        string url = _config.BaseUrl + $@"?Key={_config.Key}&Text={HttpUtility.UrlEncode(text)}&Origin=GBR&Language=en&Test=false&block=true&cache=true";

        var data = await HttpHelper.Get<SearchResultDto>(url, _config.Referer);
        if (data == null)
        {
            return Array.Empty<SearchResultItemDto>();
        }

        return data.Items;
    }

    private async Task<SearchResultItemDto[]> GetFromIdResults(string id)
    {
        string url = _config.BaseUrl + $@"?Key={_config.Key}&Text=EH39%204ER&Container={HttpUtility.UrlEncode(id)}&Origin=GBR&Limit=10&Language=en&$block=true&$cache=true";

        var data = await HttpHelper.Get<SearchResultDto>(url, _config.Referer);
        if (data == null)
        {
            return Array.Empty<SearchResultItemDto>();
        }

        return data.Items;
    }

    [Route("/Search")]
    [HttpGet("Search/{text}")]
    public async Task<IActionResult> Search(string text)
    {
        _logger.LogInformation($"Search/{text}");

        var results = await GetSearchResults(text);
        if (!results.Any())
        {
            return NotFound();
        }

        return Ok(results);
    }

    [Route("/FromId")]
    [HttpGet("FromId/{id}")]
    public async Task<IActionResult> FromId(string id)
    {
        _logger.LogInformation($"FromId/{id}");

        var results = await GetFromIdResults(id);
        if (!results.Any())
        {
            return NotFound();
        }

        return Ok(results);
    }

    [Route("/FindAddresses")]
    [HttpGet("FindAddresses/{text}")]
    public async Task<IActionResult> FinadAddress(string text)
    {
        _logger.LogInformation($"FindAddresses/{text}");

        var searchResults = await GetSearchResults(text);
        var addresses = new List<SearchResultItemDto>();

        foreach(var searchResult in searchResults)
        {
            if (!searchResult.Description.EndsWith(" - More Addresses"))
            {
                addresses.Add(searchResult);
                continue;
            }

            var moreAddresses = await GetFromIdResults(searchResult.Id);
            addresses.AddRange(moreAddresses);
        }

        if (!addresses.Any())
        {
            return NotFound();
        }

        return Ok(addresses);
    }
}
