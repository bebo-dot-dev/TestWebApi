using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TestController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpPost]
    [Route("small")]
    public IActionResult GetSmallFile(FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/small.gz", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = "small.gz" };
    }
        
    [HttpPost]
    [Route("large")]
    public IActionResult GetBigFile(FileRequest request)
    {
        var fileStream = System.IO.File.Open("./Files/large.gz", FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = "large.gz" };
    }
    
    [HttpGet]
    [Route("small-client")]
    public async Task<IActionResult> GetSmallFileClient()
    {
        var payload = new StringContent(
            JsonSerializer.Serialize(new FileRequest()),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var httpClient = _httpClientFactory.CreateClient("any-cert-proxied-client");
        
        var response = await httpClient.PostAsync(GetUri("Test/small"), payload);
        var clientStream = await response.Content.ReadAsStreamAsync();
        
        return new FileStreamResult(clientStream, "application/octet-stream") { FileDownloadName = "small.gz" };
    }
    
    [HttpGet]
    [Route("large-client")]
    public async Task<FileStreamResult> GetLargeFileClient()
    {
        var payload = new StringContent(
            JsonSerializer.Serialize(new FileRequest()),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var httpClient = _httpClientFactory.CreateClient("any-cert-proxied-client");
        
        var response = await httpClient.PostAsync(GetUri("Test/large"), payload);
        var clientStream = await response.Content.ReadAsStreamAsync();
        
        return new FileStreamResult(clientStream, "application/octet-stream") { FileDownloadName = "large.gz" };
    }

    private string GetUri(string path)
    {
        return $"{Request.Scheme}://{Request.Host}/{path}";
    }
}