using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SPAddin.Core.UrlPath.Common;
using SPAddin.Core.UrlPath.DB;
using AspNet.Core.SharePoint.Addin.Authentication.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace SPAddin.Core.UrlPath
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			if (env.IsDevelopment())
			{
				builder.AddUserSecrets();
			}

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AddInContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Addin")));

			services.AddOptions();

			services.Configure<LowTrustSettings>(Configuration.GetSection("SharePoint"));


			// Add framework services.
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				LoginPath = "/Auth/Login",
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				Events = new CustomCookieEvents(serviceProvider)
			});

			app.UseSPAddinAuthentication(new SPAddinAuthenticationOptions
			{
				AutomaticAuthenticate = false,
				AutomaticChallenge = false,
				SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
				AuthenticationScheme = SPAddinAuthenticationDefaults.AuthenticationType,
				AuthSettings = serviceProvider.GetService<IOptions<LowTrustSettings>>().Value
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "Auth",
					template: "{controller=Auth}/{action=AppRedirect}");

				routes.MapRoute(
					name: "ShortUrl",
					template: "{shortUrl}/{controller=Home}/{action=Index}");

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
