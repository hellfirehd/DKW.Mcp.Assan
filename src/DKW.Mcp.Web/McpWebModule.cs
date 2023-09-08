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

using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan;
using DKW.Mcp.Localization;
using DKW.Mcp.Web.Menus;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Mcp.Web;

[DependsOn(typeof(McpApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiThemeSharedModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiAssanModule))]
public class McpWebModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
		{
			options.AddAssemblyResource(typeof(McpResource), typeof(McpWebModule).Assembly);
		});

		PreConfigure<IMvcBuilder>(mvcBuilder =>
		{
			mvcBuilder.AddApplicationPartIfNotExists(typeof(McpWebModule).Assembly);
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new McpMenuContributor());
		});

		Configure<AbpVirtualFileSystemOptions>(options =>
		{
			options.FileSets.AddEmbedded<McpWebModule>();
		});

		context.Services.AddAutoMapperObjectMapper<McpWebModule>();
		Configure<AbpAutoMapperOptions>(options =>
		{
			options.AddMaps<McpWebModule>(validate: true);
		});

		Configure<RazorPagesOptions>(options =>
		{
			//Configure authorization.
		});
	}
}
