﻿using Umbraco.Cms.Core;

namespace Umbraco.Cms.Api.Management.ViewModels.Language;

public class LanguageResponseModel : LanguageModelBase
{
    public string IsoCode { get; set; } = string.Empty;
    public string Type => Constants.UdiEntityType.Language;
}