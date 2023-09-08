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

using DKW.Mcp.Blazor.Menus;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace DKW.Mcp.Blazor;

[DependsOn(typeof(McpApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebThemingModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class McpBlazorModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAutoMapperObjectMapper<McpBlazorModule>();

		Configure<AbpAutoMapperOptions>(options =>
		{
			options.AddProfile<McpBlazorAutoMapperProfile>(validate: true);
		});

		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new McpMenuContributor());
		});

		Configure<AbpRouterOptions>(options =>
		{
			options.AdditionalAssemblies.Add(typeof(McpBlazorModule).Assembly);
		});
	}
}
