using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using Rolodex.Web.DataStore;

namespace Rolodex.Web.Infrastructure.Tags;

public abstract class EntitySelectElementBuilder<T> : ElementTagBuilder where T : class
{
    public override bool Matches(ElementRequest subject) => typeof(T).IsAssignableFrom(subject.Accessor.PropertyType);

    public override HtmlTag Build(ElementRequest request)
    {
        var results = Source(request);

        var selectTag = new SelectTag(t =>
        {
            t.Option(string.Empty, string.Empty);
            foreach (var result in results)
            {
                BuildOptionTag(t, result, request);
            }
        });

        var entity = request.Value<T>();

        if (entity != null)
        {
            selectTag.SelectByValue(GetValue(entity));
        }

        return selectTag;
    }

    protected virtual HtmlTag BuildOptionTag(SelectTag select, T model, ElementRequest request) 
        => @select.Option(GetDisplayValue(model), GetValue(model));

    protected abstract int GetValue(T instance);
    protected abstract string GetDisplayValue(T instance);

    protected virtual IEnumerable<T> Source(ElementRequest request) 
        => request.Get<RolodexContext>().Set<T>();
}