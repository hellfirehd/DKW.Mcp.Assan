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

using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using DKW.Abp.AspNetCore.Components.Web.Assan.Themes.Assan;
using DKW.Abp.AspNetCore.Components.WebAssembly.Assan;
using DKW.Mcp.Blazor.WebAssembly;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Blazor.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;
using Volo.Abp.TenantManagement.Blazor.WebAssembly;
using Volo.Abp.UI.Navigation;

namespace DKW.Mcp.Blazor.Host;

[DependsOn(typeof(AbpAutofacWebAssemblyModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebAssemblyAssanModule))]
[DependsOn(typeof(AbpAccountApplicationContractsModule))]
[DependsOn(typeof(AbpIdentityBlazorWebAssemblyModule))]
[DependsOn(typeof(AbpTenantManagementBlazorWebAssemblyModule))]
[DependsOn(typeof(AbpSettingManagementBlazorWebAssemblyModule))]
[DependsOn(typeof(McpBlazorWebAssemblyModule))]
public class McpBlazorHostModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
		var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

		ConfigureAuthentication(builder);
		ConfigureHttpClient(context, environment);
		ConfigureBlazorise(context);
		ConfigureRouter();
		ConfigureUI(builder);
		ConfigureMenu(context);
		ConfigureAutoMapper();
	}

	private void ConfigureRouter()
	{
		Configure<AbpRouterOptions>(options =>
		{
			options.AppAssembly = typeof(McpBlazorHostModule).Assembly;
		});
	}

	private void ConfigureMenu(ServiceConfigurationContext context)
	{
		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new McpHostMenuContributor(context.Services.GetConfiguration()));
		});
	}

	private static void ConfigureBlazorise(ServiceConfigurationContext context)
	{
		context.Services
			.AddBootstrap5Providers()
			.AddFontAwesomeIcons();
	}

	private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
	{
		builder.Services.AddOidcAuthentication(options =>
		{
			builder.Configuration.Bind("AuthServer", options.ProviderOptions);
			options.ProviderOptions.DefaultScopes.Add("Mcp");
		});
	}

	private static void ConfigureUI(WebAssemblyHostBuilder builder)
	{
		builder.RootComponents.Add<App>("#ApplicationContainer");
	}

	private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
	{
		context.Services.AddTransient(sp => new HttpClient
		{
			BaseAddress = new Uri(environment.BaseAddress)
		});
	}

	private void ConfigureAutoMapper()
	{
		Configure<AbpAutoMapperOptions>(options =>
		{
			options.AddMaps<McpBlazorHostModule>();
		});
	}
}
