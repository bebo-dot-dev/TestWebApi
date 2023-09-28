using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using HttpMethod = System.Net.Http.HttpMethod;

namespace TestWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _gitHubClient;

    public TestController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _gitHubClient = httpClientFactory.CreateClient("github-client");
    }
    
    [HttpPost]
    [Route("small.gz")]
    public IActionResult GetSmallFile(FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/small.gz", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = "small.gz" };
    }
        
    [HttpPost]
    [Route("large.gz")]
    public IActionResult GetBigFile(FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/large.gz", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = "large.gz" };
    }
    
    [HttpPost]
    [Route("plain")]
    public IActionResult PostPlainFile(FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/plain", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "text/plain") { FileDownloadName = "plain" };
    }
    
    [HttpGet]
    [Route("plain")]
    public IActionResult GetPlainFile([FromQuery] FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/plain", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "text/plain") { FileDownloadName = "plain" };
    }
    
    [HttpGet]
    [Route("client.small.gz")]
    public async Task<IActionResult> GetSmallFileClient()
    {
        var clientStream = await GetClientStreamAsync("Test/small.gz");
        return new FileStreamResult(clientStream, "application/octet-stream") { FileDownloadName = "small.gz" };
    }
    
    [HttpGet]
    [Route("client.large.gz")]
    public async Task<FileStreamResult> GetLargeFileClient()
    {
        var clientStream = await GetClientStreamAsync("Test/large.gz");
        return new FileStreamResult(clientStream, "application/octet-stream") { FileDownloadName = "large.gz" };
    }
    
    [HttpGet]
    [Route("client.plain")]
    public async Task<FileStreamResult> GetPlainFileClient()
    {
        var clientStream = await GetClientStreamAsync("Test/plain");
        return new FileStreamResult(clientStream, "text/plain") { FileDownloadName = "plain" };
    }

    [HttpGet]
    [Route("github")]
    public async Task<string> DoGitHubRequest()
    {
        var result = await _gitHubClient.GetStringAsync("/TestWebApi");
        return result;
    }
    
    private string GetUri(string path)
    {
        return $"{Request.Scheme}://{Request.Host}/{path}";
    }
    
    private async Task<Stream> GetClientStreamAsync(string path)
    {
        var uri = GetUri(path);
        
        var payload = new StringContent(
            JsonSerializer.Serialize(new FileRequest()),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(uri),
            Headers = {
                { "x-version", "1" },
                { "x-id", Guid.NewGuid().ToString() }
            },
            Content = payload
        };

        var httpClient = _httpClientFactory.CreateClient("proxied-client");
        
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStreamAsync();
    }
}