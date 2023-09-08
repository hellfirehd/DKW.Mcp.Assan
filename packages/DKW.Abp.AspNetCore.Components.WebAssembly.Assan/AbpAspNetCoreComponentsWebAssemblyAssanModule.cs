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

using DKW.Abp.AspNetCore.Components.Web.Assan;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Http.Client.IdentityModel.WebAssembly;
using Volo.Abp.Modularity;

namespace DKW.Abp.AspNetCore.Components.WebAssembly.Assan;

[DependsOn(typeof(AbpAspNetCoreComponentsWebAssanModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelWebAssemblyModule))]
public class AbpAspNetCoreComponentsWebAssemblyAssanModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpRouterOptions>(options =>
		{
			options.AdditionalAssemblies.Add(typeof(AbpAspNetCoreComponentsWebAssemblyAssanModule).Assembly);
		});

		Configure<AbpToolbarOptions>(options =>
		{
			options.Contributors.Add(new AssanToolbarContributor());
		});
	}
}
