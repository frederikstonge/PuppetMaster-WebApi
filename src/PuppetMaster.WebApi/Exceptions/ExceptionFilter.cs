using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PuppetMaster.WebApi.Exceptions
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                if (context.Exception is HttpResponseException httpResponseException)
                {
                    context.Result = new ObjectResult(httpResponseException.Value)
                    {
                        StatusCode = (int)httpResponseException.StatusCode
                    };

                    context.ExceptionHandled = true;
                }

                _logger.LogError(context.Exception, "An error has occurred");
            }
        }
    }
}
