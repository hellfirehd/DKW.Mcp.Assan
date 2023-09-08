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
using DKW.Mcp.EntityFrameworkCore;
using DKW.Mcp.MultiTenancy;
using DKW.Mcp.Web;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Mcp;

[DependsOn(typeof(McpWebModule))]
[DependsOn(typeof(McpApplicationModule))]
[DependsOn(typeof(McpHttpApiModule))]
[DependsOn(typeof(McpEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpAccountWebModule))]
[DependsOn(typeof(AbpAccountApplicationModule))]
[DependsOn(typeof(AbpAccountHttpApiModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementApplicationModule))]
[DependsOn(typeof(AbpPermissionManagementHttpApiModule))]
[DependsOn(typeof(AbpIdentityWebModule))]
[DependsOn(typeof(AbpIdentityApplicationModule))]
[DependsOn(typeof(AbpIdentityHttpApiModule))]
[DependsOn(typeof(AbpIdentityEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementDomainIdentityModule))]
[DependsOn(typeof(AbpFeatureManagementWebModule))]
[DependsOn(typeof(AbpFeatureManagementApplicationModule))]
[DependsOn(typeof(AbpFeatureManagementHttpApiModule))]
[DependsOn(typeof(AbpFeatureManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpTenantManagementWebModule))]
[DependsOn(typeof(AbpTenantManagementApplicationModule))]
[DependsOn(typeof(AbpTenantManagementHttpApiModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiAssanModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
public class McpWebUnifiedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		Configure<AbpDbContextOptions>(options =>
		{
			options.UseSqlServer();
		});

		if (hostingEnvironment.IsDevelopment())
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
			{
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain.Shared", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application.Contracts", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpWebModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Web", Path.DirectorySeparatorChar)));
			});
		}

		context.Services.AddAbpSwaggerGen(
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
			options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português (Brasil)"));
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

		Configure<AbpMultiTenancyOptions>(options =>
		{
			options.IsEnabled = MultiTenancyConsts.IsEnabled;
		});

#if DEBUG
		context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
	}

	public async override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
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

		if (MultiTenancyConsts.IsEnabled)
		{
			app.UseMultiTenancy();
		}

		app.UseAbpRequestLocalization();
		app.UseAuthorization();

		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");
		});

		app.UseAuditing();
		app.UseAbpSerilogEnrichers();
		app.UseConfiguredEndpoints();

		using (var scope = context.ServiceProvider.CreateScope())
		{
			await scope.ServiceProvider
				.GetRequiredService<IDataSeeder>()
				.SeedAsync();
		}
	}
}
