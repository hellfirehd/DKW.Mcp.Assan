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

using DKW.Abp.AspNetCore.Components.Server.Assan.Bundling;
using DKW.Abp.AspNetCore.Components.Web.Assan;
using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.AspNetCore.Components.Server.Theming.Bundling;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Modularity;

namespace DKW.Abp.AspNetCore.Components.Server.Assan;

[DependsOn(typeof(AbpAspNetCoreComponentsWebAssanModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsServerThemingModule))]
public class AbpAspNetCoreComponentsServerAssanModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpToolbarOptions>(options =>
		{
			options.Contributors.Add(new AssanToolbarContributor());
		});

		Configure<AbpBundlingOptions>(options =>
		{
			options
				.StyleBundles
				.Add(BlazorAssanBundles.Styles.Global, bundle =>
				{
					bundle
						.AddBaseBundles(BlazorStandardBundles.Styles.Global)
						.AddContributors(typeof(BlazorAssanStyleContributor));
				});

			options
				.ScriptBundles
				.Add(BlazorAssanBundles.Scripts.Global, bundle =>
				{
					bundle
						.AddBaseBundles(BlazorStandardBundles.Scripts.Global)
						.AddContributors(typeof(BlazorAssanScriptContributor));
				});
		});
	}
}
