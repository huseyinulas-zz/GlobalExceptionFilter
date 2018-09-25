using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Core
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
            logger.LogError(new EventId(context.Exception.HResult),
                            context.Exception,
                            context.Exception.Message);

            if (context.Exception.GetType() == typeof(CustomApiException))
            {
                var exception = (CustomApiException)context.Exception;
                exception.ProblemDetails.Instance = context.HttpContext.Request.Path;

                context.Result = new ObjectResult(exception.ProblemDetails);
                context.HttpContext.Response.StatusCode = exception.ProblemDetails.Status.Value;
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
    }
}
