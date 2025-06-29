using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;

namespace netapi_template.Filters;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // Check if this is a FluentValidation AsyncValidatorInvokedSynchronouslyException
        if (context.Exception is AsyncValidatorInvokedSynchronouslyException)
        {
            // We're going to handle this exception by doing nothing
            // This will allow the controller action to execute normally
            context.ExceptionHandled = true;
        }
    }
}
