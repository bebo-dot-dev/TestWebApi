using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using TestWebApi.Extensions;
using TestWebApi.Filters;
using TestWebApi.Validation;

namespace TestWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
                options.Filters.Add<ExceptionFilter>();                
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
                        
            services.AddSwaggerGen();
            
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx =>
                {
                    var loggerFactory = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ModelStateErrorDetailsResult>();
                    ctx.HttpContext.Response.Headers["cache-control"] = "no-store,no-cache";
                    ctx.HttpContext.Response.Headers["pragma"] = "no-cache";
                    return new ModelStateErrorDetailsResult(logger);
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {                    
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var errorDetail = env.IsDevelopment()
                        ? exception.ToString()
                        : $"urn:BSS:TraceIdentifier:{context.TraceIdentifier}";

                    var problemDetails = new ProblemDetails
                    {
                        Instance = $"urn:BSS:TraceIdentifier:{context.TraceIdentifier}",
                        Title = "An unhandled exception was caught",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Type = exception.GetType().Name,
                        Detail = errorDetail
                    };
                                                                                                                        
                    context.Response.StatusCode = problemDetails.Status.Value;
                    await context.Response.WriteJson(problemDetails, HttpStatusCode.InternalServerError, "application/problem+json");
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });            
        }
    }
}
