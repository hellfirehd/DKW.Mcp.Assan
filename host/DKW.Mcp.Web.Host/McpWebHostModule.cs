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
using DKW.Mcp.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Mcp;

[DependsOn(typeof(McpWebModule))]
[DependsOn(typeof(McpHttpApiClientModule))]
[DependsOn(typeof(McpHttpApiModule))]
[DependsOn(typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule))]
[DependsOn(typeof(AbpAspNetCoreMvcClientModule))]
[DependsOn(typeof(AbpHttpClientWebModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiAssanModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelWebModule))]
[DependsOn(typeof(AbpIdentityWebModule))]
[DependsOn(typeof(AbpIdentityHttpApiClientModule))]
[DependsOn(typeof(AbpFeatureManagementWebModule))]
[DependsOn(typeof(AbpFeatureManagementHttpApiClientModule))]
[DependsOn(typeof(AbpTenantManagementWebModule))]
[DependsOn(typeof(AbpTenantManagementHttpApiClientModule))]
[DependsOn(typeof(AbpPermissionManagementHttpApiClientModule))]
[DependsOn(typeof(AbpSettingManagementHttpApiClientModule))]
[DependsOn(typeof(AbpSettingManagementWebModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
public class McpWebHostModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
		{
			options.AddAssemblyResource(
				typeof(McpResource),
				typeof(McpDomainSharedModule).Assembly,
				typeof(McpApplicationContractsModule).Assembly,
				typeof(McpWebHostModule).Assembly
			);
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		ConfigureMenu(configuration);
		ConfigureCache();
		ConfigureUrls(configuration);
		ConfigureAuthentication(context, configuration);
		ConfigureAutoMapper();
		ConfigureVirtualFileSystem(hostingEnvironment);
		ConfigureSwaggerServices(context.Services);
		ConfigureMultiTenancy();
		ConfigureDataProtection(context, configuration, hostingEnvironment);
	}

	private void ConfigureMenu(IConfiguration configuration)
	{
		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new McpWebHostMenuContributor(configuration));
		});
	}

	private void ConfigureCache()
	{
		Configure<AbpDistributedCacheOptions>(options =>
		{
			options.KeyPrefix = "Mcp:";
		});
	}

	private void ConfigureUrls(IConfiguration configuration)
	{
		Configure<AppUrlOptions>(options =>
		{
			options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
		});
	}

	private void ConfigureMultiTenancy()
	{
		Configure<AbpMultiTenancyOptions>(options =>
		{
			options.IsEnabled = true;
		});
	}

	private static void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
	{
		context.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = "Cookies";
				options.DefaultChallengeScheme = "oidc";
			})
			.AddCookie("Cookies", options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromDays(365);
			})
			.AddAbpOpenIdConnect("oidc", options =>
			{
				options.Authority = configuration["AuthServer:Authority"];
				options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
				options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

				options.ClientId = configuration["AuthServer:ClientId"];

				options.SaveTokens = true;
				options.GetClaimsFromUserInfoEndpoint = true;

				options.Scope.Add("roles");
				options.Scope.Add("email");
				options.Scope.Add("phone");
				options.Scope.Add("Mcp");
			});
	}

	private void ConfigureAutoMapper()
	{
		Configure<AbpAutoMapperOptions>(options =>
		{
			options.AddMaps<McpWebHostModule>();
		});
	}

	private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
	{
		if (hostingEnvironment.IsDevelopment())
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
			{
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain.Shared", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application.Contracts", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpWebModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Web", Path.DirectorySeparatorChar)));
			});
		}
	}

	private static void ConfigureSwaggerServices(IServiceCollection services)
	{
		services.AddAbpSwaggerGen(
			options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mcp API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.CustomSchemaIds(type => type.FullName);
			}
		);
	}

	private static void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
	{
		var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Mcp");
		if (!hostingEnvironment.IsDevelopment())
		{
			var connectionString = configuration["Redis:Configuration"]
				?? throw new AbpException("Mcp_Web:RootUrl has not been defined.");
			var redis = ConnectionMultiplexer.Connect(connectionString);
			dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Mcp-Protection-Keys");
		}
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var app = context.GetApplicationBuilder();
		var env = context.GetEnvironment();

		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseErrorPage();
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();

		app.UseMultiTenancy();

		app.UseAbpRequestLocalization();
		app.UseAuthorization();

		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mcp API");
		});

		app.UseAuditing();
		app.UseAbpSerilogEnrichers();
		app.UseConfiguredEndpoints();
	}
}
