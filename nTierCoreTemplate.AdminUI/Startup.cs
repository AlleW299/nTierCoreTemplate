using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nTierCoreTemplate.Admin.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nTierCoreTemplate.Core.Models;
using nTierCoreTemplate.Core.Repositories;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace nTierCoreTemplate.AdminUI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//Provide a secret key to Encrypt and Decrypt the Token
			//var SecretKey = Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");

			// Add localization
			services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
			services.Configure<RequestLocalizationOptions>(options =>
			{
				CultureInfo[] supportedCultures = new[]
				{
					new CultureInfo("de_DE"),
					new CultureInfo("en-US"),
					new CultureInfo("sv-SE")
				};

				options.DefaultRequestCulture = new RequestCulture("en-US");
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
			});


			// Set up definition for Controller project and MVC
			var controllerAssembly = Assembly.Load(new AssemblyName("nTierCoreTemplate.Admin"));
			services.AddMvc(options =>
			{
				options.EnableEndpointRouting = false;
			})
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddApplicationPart(controllerAssembly)
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddControllersAsServices();

			// Adds a default in-memory implementation of IDistributedCache.
			services.AddDistributedMemoryCache();

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromSeconds(3600);
			});

			// Configure strongly typed settings objects.
			// Make sure AppSettings Object match proper appsettings.json attributes.
			
			var appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSetting>(appSettingsSection);

			// configure jwt authentication
			var appSettings = appSettingsSection.Get<AppSetting>();
			var key = Encoding.ASCII.GetBytes(appSettings.Secret);
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(token =>
			{
				token.RequireHttpsMetadata = false;
				token.SaveToken = true;
				token.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = "http://localhost:44335/",
					ValidateAudience = true,
					ValidAudience = "http://localhost:44335/",
					RequireExpirationTime = true,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};
			});

			// configure depemdecy injection for application services
			services.AddHttpContextAccessor();
			services.AddScoped<IAPIAuthenticationService, APIAuthenticationService>();
			services.AddScoped<ITokenProviderService, TokenProviderService>();
			services.AddScoped<I_PreRequsitsService, _PreRequsitsService>();

			services.Configure<DataConnection>(Configuration.GetSection("DataConnection"));
			services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

			services.AddScoped<I_SettingService, _SettingService>();
			services.AddScoped<I_UserService, _UserService>();

			services.AddScoped<I_UserRepository, _UserRepository>(_ => new _UserRepository(Configuration.GetSection("DataConnection").Value));
			services.AddScoped<I_RoleRepository, _RoleRepository>(_ => new _RoleRepository(Configuration.GetSection("DataConnection").Value));
			services.AddScoped<I_UserRoleRepository, _UserRoleRepository>(_ => new _UserRoleRepository(Configuration.GetSection("DataConnection").Value));
			services.AddScoped<I_SettingRepository, _SettingRepository>(_ => new _SettingRepository(Configuration.GetSection("DataConnection").Value));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Localization
			app.UseRequestLocalization();

			// Force Https for all APIs
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			// add User session
			app.UseSession();

			// Add JWToken to all incoming HTTP Request Header
			app.Use(async (context, next) =>
			{
				var JWToken = context.Session.GetString("JWToken");
				if (!string.IsNullOrEmpty(JWToken))
				{
					context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
				}
				await next();
			});


			app.UseAuthentication();

			// Add JWToken Authentication service
			app.UseAuthorization();

			app.UseMvc(routes =>
			{
			routes
			.MapRoute(
				name: "default",
				template: "{controller=Home}/{action=Index}/{id?}")
			.MapRoute(
				name: "Simple",
				template: "{controller}");
			});


		}
	}
}
