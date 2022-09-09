using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Rolodex.Pages;

public static class PageModelExtensions
{
    public static ActionResult RedirectToPageJson<TPage>(this TPage controller, string pageName)
        where TPage : PageModel =>
        controller.JsonNet(new
            {
                redirect = controller.Url.Page(pageName)
            }
        );

    public static ContentResult JsonNet(this PageModel controller, object model)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        var serialized = JsonSerializer.Serialize(model, options);

        return new ContentResult
        {
            Content = serialized,
            ContentType = "application/json"
        };
    }

}