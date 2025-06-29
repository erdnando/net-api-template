using Microsoft.AspNetCore.Mvc.Filters;

namespace netapi_template.Attributes;

/// <summary>
/// Disables automatic model validation for action methods
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class DisableValidationAttribute : Attribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Clear any model state errors
        context.ModelState.Clear();
    }
}
