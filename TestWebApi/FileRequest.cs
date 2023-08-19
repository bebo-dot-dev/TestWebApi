namespace TestWebApi;

public class FileRequest
{
    public string ContentType { get; set; } = "test-content-type";
    
    public string PackageName { get; set; } = "test-package-name";

    public string Version { get; set; } = "v1.0";

    public string FileName { get; set; } = "test-file.json";
}