using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestWebApi.Extensions
{
    public static class HttpExtensions
    {
        public static async Task WriteJson<T>(this HttpResponse response, T obj, HttpStatusCode httpStatusCode, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json";
            response.StatusCode = (int)httpStatusCode;

            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            await response.Body.WriteAsync(jsonBytes);
        }
    }
}
