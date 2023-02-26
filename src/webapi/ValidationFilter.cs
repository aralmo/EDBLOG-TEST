using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid == false)
        {
            var errors = MapErrors(context.ModelState);
            context.Result = new BadRequestObjectResult(new
            {
                Errors = errors.ToArray()
            });
        }
    }

    static IEnumerable<object> MapErrors(ModelStateDictionary modelState)
    {
        foreach(var item in modelState)
            if (item.Value != null)
                foreach(var error in item.Value.Errors)
                    yield return new { Key = item.Key, Error = error.ErrorMessage };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do something after the action executes.
    }
}