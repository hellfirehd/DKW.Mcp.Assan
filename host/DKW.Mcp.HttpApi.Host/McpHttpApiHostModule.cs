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

using DKW.Mcp.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Mcp;

[DependsOn(typeof(McpApplicationModule))]
[DependsOn(typeof(McpEntityFrameworkCoreModule))]
[DependsOn(typeof(McpHttpApiModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiMultiTenancyModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
public class McpHttpApiHostModule : AbpModule
{

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		Configure<AbpDbContextOptions>(options =>
		{
			options.UseSqlServer();
		});

		Configure<AbpMultiTenancyOptions>(options =>
		{
			options.IsEnabled = true;
		});

		if (hostingEnvironment.IsDevelopment())
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
			{
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain.Shared", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application.Contracts", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application", Path.DirectorySeparatorChar)));
			});
		}

		context.Services.AddAbpSwaggerGenWithOAuth(
			configuration["AuthServer:Authority"],
			new Dictionary<String, String>
			{
				{"Mcp", "Mcp API"}
			},
			options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mcp API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.CustomSchemaIds(type => type.FullName);
			});

		Configure<AbpLocalizationOptions>(options =>
		{
			options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
			options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
			options.Languages.Add(new LanguageInfo("en", "en", "English"));
			options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
			options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
			options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
			options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
			options.Languages.Add(new LanguageInfo("is", "is", "Icelandic", "is"));
			options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
			options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
			options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
			options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română"));
			options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
			options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
			options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
			options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
			options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
			options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
			options.Languages.Add(new LanguageInfo("es", "es", "Español"));
			options.Languages.Add(new LanguageInfo("el", "el", "Ελληνικά"));
		});

		context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = configuration["AuthServer:Authority"];
				options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
				options.Audience = "Mcp";
			});

		Configure<AbpDistributedCacheOptions>(options =>
		{
			options.KeyPrefix = "Mcp:";
		});

		var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Mcp");
		if (!hostingEnvironment.IsDevelopment())
		{
			var connectionString = configuration["Redis:ConnectionString"]
				?? throw new AbpException("Redis:ConnectionString has not been defined.");
			var redis = ConnectionMultiplexer.Connect(connectionString);
			dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Mcp-Protection-Keys");
		}

		context.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder
					.WithOrigins(
						configuration["App:CorsOrigins"]?
							.Split(",", StringSplitOptions.RemoveEmptyEntries)
							.Select(o => o.RemovePostFix("/"))
							.ToArray() ?? Array.Empty<String>()
					)
					.WithAbpExposedHeaders()
					.SetIsOriginAllowedToAllowWildcardSubdomains()
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
			});
		});
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
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseCorrelationId();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseCors();
		app.UseAuthentication();
		app.UseMultiTenancy();
		app.UseAbpRequestLocalization();
		app.UseAuthorization();
		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");

			var configuration = context.GetConfiguration();
			options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
			options.OAuthScopes("Mcp");
		});
		app.UseAuditing();
		app.UseAbpSerilogEnrichers();
		app.UseConfiguredEndpoints();
	}
}
