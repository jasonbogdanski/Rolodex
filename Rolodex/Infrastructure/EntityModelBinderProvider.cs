using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rolodex.Models;

namespace Rolodex.Infrastructure;

public class EntityModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context) 
        => (typeof(IEntity).IsAssignableFrom(context.Metadata.ModelType) ? new EntityModelBinder() : null)!;
}