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
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Themes.Assan.Components.Toolbar.UserMenu;

public class UserMenuViewComponent : AbpViewComponent
{
	protected IMenuManager MenuManager { get; }

	public UserMenuViewComponent(IMenuManager menuManager)
	{
		MenuManager = menuManager;
	}

	public virtual async Task<IViewComponentResult> InvokeAsync()
	{
		var menu = await MenuManager.GetAsync(StandardMenus.User);
		return View("~/Themes/Assan/Components/Toolbar/UserMenu/Default.cshtml", menu);
	}
}
