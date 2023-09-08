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
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Themes.Assan.Components.PageAlerts;

public class PageAlertsViewComponent : AbpViewComponent
{
	protected IAlertManager AlertManager { get; }

	public PageAlertsViewComponent(IAlertManager alertManager)
	{
		AlertManager = alertManager;
	}

	public IViewComponentResult Invoke(String name)
	{
		return View("~/Themes/Assan/Components/PageAlerts/Default.cshtml", AlertManager.Alerts);
	}
}
