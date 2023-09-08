// MCP Application Suite
// Copyright (C) 2023 Doug Wilson
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Themes.Assan.Components.Toolbar.LanguageSwitch;

public class LanguageSwitchViewComponent : AbpViewComponent
{
	protected ILanguageProvider LanguageProvider { get; }

	public LanguageSwitchViewComponent(ILanguageProvider languageProvider)
	{
		LanguageProvider = languageProvider;
	}

	public virtual async Task<IViewComponentResult> InvokeAsync()
	{
		var languages = await LanguageProvider.GetLanguagesAsync();
		var currentLanguage = languages.FindByCulture(
			CultureInfo.CurrentCulture.Name,
			CultureInfo.CurrentUICulture.Name
		);

		if (currentLanguage == null)
		{
			var abpRequestLocalizationOptionsProvider = HttpContext.RequestServices.GetRequiredService<IAbpRequestLocalizationOptionsProvider>();
			var localizationOptions = await abpRequestLocalizationOptionsProvider.GetLocalizationOptionsAsync();
			currentLanguage = localizationOptions.DefaultRequestCulture == null
				? new LanguageInfo(
					CultureInfo.CurrentCulture.Name,
					CultureInfo.CurrentUICulture.Name,
					CultureInfo.CurrentUICulture.DisplayName)
				: new LanguageInfo(
					localizationOptions.DefaultRequestCulture.Culture.Name,
					localizationOptions.DefaultRequestCulture.UICulture.Name,
					localizationOptions.DefaultRequestCulture.UICulture.DisplayName);
		}

		var model = new LanguageSwitchViewComponentModel
		{
			CurrentLanguage = currentLanguage,
			OtherLanguages = languages.Where(l => l != currentLanguage).ToList()
		};

		return View("~/Themes/Assan/Components/Toolbar/LanguageSwitch/Default.cshtml", model);
	}
}
