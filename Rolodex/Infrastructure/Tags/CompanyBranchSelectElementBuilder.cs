using Rolodex.Models;

namespace Rolodex.Infrastructure.Tags;

public class CompanyBranchSelectElementBuilder : EntitySelectElementBuilder<CompanyBranch>
{
    protected override int GetValue(CompanyBranch instance) => instance.Id;

    protected override string GetDisplayValue(CompanyBranch instance) => instance.Name;
}