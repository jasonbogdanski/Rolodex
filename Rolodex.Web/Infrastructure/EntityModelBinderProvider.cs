using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rolodex.Web.Models;

namespace Rolodex.Web.Infrastructure;

public class EntityModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context) 
        => (typeof(IEntity).IsAssignableFrom(context.Metadata.ModelType) ? new EntityModelBinder() : null)!;
}