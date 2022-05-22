using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TestWebApi.Extensions;
using TestWebApi.Models.Response;

namespace TestWebApi.Validation
{
    public class ModelStateErrorDetailsResult : IActionResult
    {
        private readonly ILogger<ModelStateErrorDetailsResult> _logger;

        public ModelStateErrorDetailsResult(ILogger<ModelStateErrorDetailsResult> logger)
        {
            _logger = logger;
        }

        async Task IActionResult.ExecuteResultAsync(ActionContext context)
        {
            var modelStateErrors = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ErrorDetail>();

            var details = "See Errors for details";
            var exceptionMessage = string.Empty;

            if (modelStateErrors.Any())
            {
                if (modelStateErrors.Length == 1 && modelStateErrors[0].Value.Errors.Count == 1 && modelStateErrors[0].Key == string.Empty)
                {
                    details = modelStateErrors[0].Value.Errors[0].ErrorMessage;
                    exceptionMessage = details;
                }
                else
                {
                    foreach (var modelStateEntry in modelStateErrors)
                    {
                        foreach (var modelStateError in modelStateEntry.Value.Errors)
                        {
                            var error = new ErrorDetail
                            {
                                Name = modelStateEntry.Key,
                                Description = modelStateError.ErrorMessage
                            };

                            errors.Add(error);
                        }
                    }
                    exceptionMessage = JsonSerializer.Serialize(errors);
                }
            }
            
            var traceIdentifier = context.HttpContext.TraceIdentifier;            
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            var problemDetails = new BaseResponse
            {                                
                Instance = $"urn:BSS:TraceIdentifier:{traceIdentifier}",
                Title = "Request Validation Error",
                Status = (int)HttpStatusCode.BadRequest,
                Type = context.ActionDescriptor.Parameters.FirstOrDefault()?.ParameterType?.Name,
                Detail = details,
                Errors = errors
            };
                        
            var badRequestException = new BadRequestException(exceptionMessage) 
            {
                Source = $"{controllerName}.{actionName}",
                HelpLink = problemDetails.Instance,
                HResult = problemDetails.Status.Value
            };
            _logger.LogError(badRequestException, $"Caught validation BadRequestException in {controllerName}.{actionName} logged with traceIdentifier {traceIdentifier}");

            await context.HttpContext.Response.WriteJson(problemDetails, HttpStatusCode.BadRequest, "application/problem+json");
        }
    }
}
