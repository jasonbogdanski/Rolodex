﻿using HtmlTags;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Rolodex.Web.Infrastructure.Tags;

[HtmlTargetElement("display-label-tag", Attributes = nameof(For), TagStructure = TagStructure.WithoutEndTag)]
public class DisplayLabelTagHelper : HtmlTagTagHelper
{
    protected override string Category => nameof(TagConventions.DisplayLabels);
}