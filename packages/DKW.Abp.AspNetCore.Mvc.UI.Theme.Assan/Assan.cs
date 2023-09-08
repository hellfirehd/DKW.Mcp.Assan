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

using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan;

[ThemeName(Name)]
public class Assan : ITheme, ITransientDependency
{
	public const String Name = "Basic";

	public virtual String? GetLayout(String name, Boolean fallbackToDefault = true)
	{
		return name switch
		{
			StandardLayouts.Application => "~/Themes/Assan/Layouts/Application.cshtml",
			StandardLayouts.Account => "~/Themes/Assan/Layouts/Account.cshtml",
			StandardLayouts.Empty => "~/Themes/Assan/Layouts/Empty.cshtml",
			_ => fallbackToDefault ? "~/Themes/Assan/Layouts/Application.cshtml" : null,
		};
	}
}
