using ProfileAPI.ApplicationCore.Exceptions;
using ProfileAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProfileAPI.Api.Filters;

public class DomainExceptionExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        context.Result = exception switch
        {
            ArgumentException ex => new BadRequestObjectResult(exception.Message),
            DomainException ex => CreateError(ErrorResponseBody.Create(ex.Code, ex.Data)),
            _ => throw new Exception(nameof(DomainExceptionExceptionFilter), exception),
        };
    }

    private static IActionResult CreateError(ErrorResponseBody error)
        => new BadRequestObjectResult(error);
}
