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

using DKW.Abp.AspNetCore.Components.Server.Assan.Themes.Assan;
using DKW.Abp.AspNetCore.Components.Server.Assan.Themes.Basic;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;

namespace DKW.Abp.AspNetCore.Components.Server.Assan;

public class AssanToolbarContributor : IToolbarContributor
{
	public Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
	{
		if (context.Toolbar.Name == StandardToolbars.Main)
		{
			context.Toolbar.Items.Add(new ToolbarItem(typeof(LoginDisplay)));
			context.Toolbar.Items.Add(new ToolbarItem(typeof(LanguageSwitch)));
		}

		return Task.CompletedTask;
	}
}
