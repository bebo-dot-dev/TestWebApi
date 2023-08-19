using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("any-cert-proxied-client", client =>
    {
        client.DefaultRequestHeaders.TransferEncodingChunked = true;
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        //route to a local proxy
        Proxy = new WebProxy("127.0.0.1:8080"),
        //ignore cert issues
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();