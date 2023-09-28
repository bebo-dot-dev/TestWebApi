using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(r => r.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);
});

builder.Services.AddHttpClient("proxied-client")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        //route to a local proxy
        Proxy = new WebProxy("127.0.0.1:8080")
    });

builder.Services.AddHttpClient("github-client", (_, client) =>
    {
        client.BaseAddress = new Uri("https://github.com/bebo-dot-dev");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();