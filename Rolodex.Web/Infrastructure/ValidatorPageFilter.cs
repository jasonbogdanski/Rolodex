using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rolodex.Web.Infrastructure;

public class ValidatorPageFilter : IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        if (context.HttpContext.Request.Method == "GET")
        {
            var result = new BadRequestResult();
            context.Result = result;
        }
        else
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var result = new ContentResult();
            var content = JsonSerializer.Serialize(context.ModelState, options);
            result.Content = content;
            result.ContentType = "application/json";

            context.HttpContext.Response.StatusCode = 400;
            context.Result = result;
        }
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }
}