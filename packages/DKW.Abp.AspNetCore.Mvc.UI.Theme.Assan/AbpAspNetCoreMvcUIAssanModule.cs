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

using DKW.Abp.AspNetCore.Components.Server.Assan;
using DKW.Abp.AspNetCore.Components.Web.Assan;
using DKW.Abp.AspNetCore.Components.WebAssembly.Assan;
using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Bundling;
using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Toolbars;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan;

[DependsOn(typeof(AbpAspNetCoreMvcUiThemeSharedModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiMultiTenancyModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebAssemblyAssanModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebAssanModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsServerAssanModule))]
public class AbpAspNetCoreMvcUiAssanModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		PreConfigure<IMvcBuilder>(mvcBuilder =>
		{
			mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAspNetCoreMvcUiAssanModule).Assembly);
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpThemingOptions>(options =>
		{
			options.Themes.Add<Assan>();

			options.DefaultThemeName ??= Assan.Name;
		});

		Configure<AbpVirtualFileSystemOptions>(options =>
		{
			options.FileSets.AddEmbedded<AbpAspNetCoreMvcUiAssanModule>("DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan");
		});

		Configure<AbpToolbarOptions>(options =>
		{
			options.Contributors.Add(new AssanMainTopToolbarContributor());
		});

		Configure<AbpBundlingOptions>(options =>
		{
			options
				.StyleBundles
				.Add(AssanBundles.Styles.Global, bundle =>
				{
					bundle
						.AddBaseBundles(StandardBundles.Styles.Global)
						.AddContributors(typeof(AssanGlobalStyleContributor));
				});

			options
				.ScriptBundles
				.Add(AssanBundles.Scripts.Global, bundle =>
				{
					bundle
						.AddBaseBundles(StandardBundles.Scripts.Global)
						.AddContributors(typeof(AssanGlobalScriptContributor));
				});
		});
	}
}
