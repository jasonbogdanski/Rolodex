using Rolodex.Web.Models;

namespace Rolodex.Web.Infrastructure.Tags;

public class CompanyBranchSelectElementBuilder : EntitySelectElementBuilder<CompanyBranch>
{
    protected override int GetValue(CompanyBranch instance) => instance.Id;

    protected override string GetDisplayValue(CompanyBranch instance) => instance.Name;
}