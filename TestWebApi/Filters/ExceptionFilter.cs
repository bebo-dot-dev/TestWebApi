using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace TestWebApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {       
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        void IExceptionFilter.OnException(ExceptionContext context)
        {
            var traceIdentifier = context.HttpContext.TraceIdentifier;
            var exception = context.Exception;
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            _logger.LogError(exception, $"Caught exception in {controllerName}.{actionName} logged with traceIdentifier {traceIdentifier}");
        }
    }
}
