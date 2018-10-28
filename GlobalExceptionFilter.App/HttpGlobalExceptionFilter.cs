using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace App
{
    public partial class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            Log(context);

            if (context.Exception is CustomApiException customApiException)
            {
                customApiException.ProblemDetails.Instance = context.HttpContext.Request.Path;

                context.Result = new ObjectResult(customApiException.ProblemDetails);
                context.HttpContext.Response.StatusCode = customApiException.ProblemDetails.Status.Value;
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "An error occurred while processing your request" }
                };

                if (env.IsDevelopment())
                {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }

        private void Log(ExceptionContext context)
        {
            if (context.Exception is CustomApiException customApiException)
            {
                if (customApiException.ProblemDetails.Status < (int)HttpStatusCode.InternalServerError && customApiException.ProblemDetails.Status >= (int)HttpStatusCode.BadRequest)
                {
                    logger.LogInformation(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                }
                else
                {
                    logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                }
            }
            else
            {
                logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
            }
        }
    }
}
