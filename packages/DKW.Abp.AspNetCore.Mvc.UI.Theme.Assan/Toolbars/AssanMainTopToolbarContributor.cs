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

using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Themes.Assan.Components.Toolbar.LanguageSwitch;
using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Themes.Assan.Components.Toolbar.UserMenu;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Localization;
using Volo.Abp.Users;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Toolbars;

public class AssanMainTopToolbarContributor : IToolbarContributor
{
	public async Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
	{
		if (context.Toolbar.Name != StandardToolbars.Main)
		{
			return;
		}

		if (context.Theme is not Assan)
		{
			return;
		}

		var languageProvider = context.ServiceProvider.GetRequiredService<ILanguageProvider>();

		//TODO: This duplicates GetLanguages() usage. Can we eleminate this?
		var languages = await languageProvider.GetLanguagesAsync();
		if (languages.Count > 1)
		{
			context.Toolbar.Items.Add(new ToolbarItem(typeof(LanguageSwitchViewComponent)));
		}

		if (context.ServiceProvider.GetRequiredService<ICurrentUser>().IsAuthenticated)
		{
			context.Toolbar.Items.Add(new ToolbarItem(typeof(UserMenuViewComponent)));
		}
	}
}
