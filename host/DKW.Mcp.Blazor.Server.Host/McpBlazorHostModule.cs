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
using DKW.Abp.AspNetCore.Components.Server.Assan;
using DKW.Abp.AspNetCore.Components.Server.Assan.Bundling;
using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan;
using DKW.Abp.AspNetCore.Mvc.UI.Theme.Assan.Bundling;
using DKW.Mcp.Blazor.Server.Host.Menus;
using DKW.Mcp.EntityFrameworkCore;
using DKW.Mcp.Localization;
using DKW.Mcp.MultiTenancy;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Blazor.Server;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Blazor.Server;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Server;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace DKW.Mcp.Blazor.Server.Host;

[DependsOn(typeof(McpEntityFrameworkCoreModule))]
[DependsOn(typeof(McpApplicationModule))]
[DependsOn(typeof(McpHttpApiModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiAssanModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpAccountWebOpenIddictModule))]
[DependsOn(typeof(AbpAccountApplicationModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsServerAssanModule))]
[DependsOn(typeof(AbpIdentityApplicationModule))]
[DependsOn(typeof(AbpIdentityEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpIdentityBlazorServerModule))]
[DependsOn(typeof(AbpFeatureManagementApplicationModule))]
[DependsOn(typeof(AbpFeatureManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpTenantManagementBlazorServerModule))]
[DependsOn(typeof(AbpTenantManagementApplicationModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementDomainIdentityModule))]
[DependsOn(typeof(AbpPermissionManagementApplicationModule))]
[DependsOn(typeof(AbpSettingManagementBlazorServerModule))]
[DependsOn(typeof(AbpSettingManagementApplicationModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(McpBlazorServerModule))]
public class McpBlazorHostModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
		{
			options.AddAssemblyResource(
				typeof(McpResource),
				typeof(McpDomainModule).Assembly,
				typeof(McpDomainSharedModule).Assembly,
				typeof(McpApplicationModule).Assembly,
				typeof(McpApplicationContractsModule).Assembly,
				typeof(McpBlazorHostModule).Assembly
			);
		});

		PreConfigure<OpenIddictBuilder>(builder =>
		{
			builder.AddValidation(options =>
			{
				options.AddAudiences("Mcp");
				options.UseLocalServer();
				options.UseAspNetCore();
			});
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

		Configure<AbpDbContextOptions>(options =>
		{
			options.UseSqlServer();
		});

		Configure<AbpBundlingOptions>(options =>
		{
			// MVC UI
			options.StyleBundles.Configure(
			AssanBundles.Styles.Global,
			bundle =>
			{
				bundle.AddFiles("/global-styles.css");
			}
		);

			//BLAZOR UI
			options.StyleBundles.Configure(
			BlazorAssanBundles.Styles.Global,
			bundle =>
			{
				bundle.AddFiles("/blazor-global-styles.css");
				//You can remove the following line if you don't use Blazor CSS isolation for components
				bundle.AddFiles("/DKW.Mcp.Blazor.Server.Host.styles.css");
			}
		);
		});

		if (hostingEnvironment.IsDevelopment())
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
			{
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain.Shared", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Domain", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application.Contracts", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, String.Format("..{0}..{0}src{0}DKW.Mcp.Application", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<McpBlazorHostModule>(hostingEnvironment.ContentRootPath);
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
			options.Languages.Add(new LanguageInfo("it", "it", "Italian", "it"));
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

		context.Services
			.AddBootstrap5Providers()
			.AddFontAwesomeIcons();

		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new McpMenuContributor());
		});

		Configure<AbpRouterOptions>(options =>
		{
			options.AppAssembly = typeof(McpBlazorHostModule).Assembly;
		});

		Configure<AppUrlOptions>(options =>
		{
			options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
			options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<String>());
		});

#if DEBUG
		context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		var env = context.GetEnvironment();
		var app = context.GetApplicationBuilder();

		app.UseAbpRequestLocalization();

		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseCorrelationId();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAbpOpenIddictValidation();

		if (MultiTenancyConsts.IsEnabled)
		{
			app.UseMultiTenancy();
		}

		app.UseUnitOfWork();
		app.UseAuthorization();
		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mcp API");
		});
		app.UseConfiguredEndpoints();

		using (var scope = context.ServiceProvider.CreateScope())
		{
			await scope.ServiceProvider
				.GetRequiredService<IDataSeeder>()
				.SeedAsync();
		}
	}
}
