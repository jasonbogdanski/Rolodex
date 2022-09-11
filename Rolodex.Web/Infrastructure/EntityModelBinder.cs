using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rolodex.Web.DataStore;

namespace Rolodex.Web.Infrastructure;

public class EntityModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var original = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (original != ValueProviderResult.None)
        {
            var originalValue = original.FirstValue;
            if (int.TryParse(originalValue, out var id))
            {
                var dbContext = bindingContext.HttpContext.RequestServices.GetRequiredService<RolodexContext>();

                var entity = await dbContext.FindAsync(bindingContext.ModelType, id);

                bindingContext.Result = entity != null ? ModelBindingResult.Success(entity) : bindingContext.Result;
            }
        }

    }
}