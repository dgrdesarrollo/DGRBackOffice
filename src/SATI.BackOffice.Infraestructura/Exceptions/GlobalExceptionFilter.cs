namespace SATI.BackOffice.Infraestructura.Exceptions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using SATI.BackOffice.Infraestructura.Entidades.Comunes;
    using SATI.BackOffice.Infraestructura.Extensions;
    using SATI.BackOffice.Infraestructura.Intefaces;
    using System;
    using System.Net;

    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IExceptionManager _ex;
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(IExceptionManager exceptionManager, ILogger<GlobalExceptionFilter> logger)
        {
            _ex = exceptionManager;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new ExceptionFormatterExtension(context.Exception).GetValue());
            Exception exception = _ex.HandleException(context.Exception);
            ExceptionValidation validation;

            if (exception is SATIException)
            {
                validation = new ExceptionValidation
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Detail = exception.Message,
                };

                context.Result = new BadRequestObjectResult(new { error = new[] { validation } });
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
            }

            if (exception is NotFoundException)
            {

                validation = new ExceptionValidation
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Not Found",
                    Detail = exception.Message,
                };


                context.Result = new NotFoundObjectResult(new { error = new[] { validation } });
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.ExceptionHandled = true;
            }

            
            validation = new ExceptionValidation
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflict",
                Detail = $"{exception.Message} Si el mismo persiste, avise al Administrador del Sistema."

            };

            context.Result = new ConflictObjectResult(new { error = new[] { validation } });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.ExceptionHandled = true;
        }
    }
}
