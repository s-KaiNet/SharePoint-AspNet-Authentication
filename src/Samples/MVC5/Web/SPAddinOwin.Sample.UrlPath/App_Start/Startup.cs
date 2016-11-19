using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Owin.SharePoint.Addin.Auth;
using AspNet.Owin.SharePoint.Addin.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.SharePoint.Client;
using Owin;
using SPAddinOwin.Data.DB;
using SPAddinOwin.Sample.UrlPath;
using SPAddinOwin.Sample.UrlPath.Common;

[assembly: OwinStartup(typeof(Startup))]

namespace SPAddinOwin.Sample.UrlPath
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.CreatePerOwinContext(AddInContext.Create);

			var cookieAuth = new CookieAuthenticationOptions
			{
				LoginPath = new PathString("/Auth/Login"),
				Provider = new AdddInCookieAuthenticationProvider()
			};

			app.SetDefaultSignInAsAuthenticationType(cookieAuth.AuthenticationType);
			app.UseCookieAuthentication(cookieAuth);

			app.UseSPAddinAuthentication(new SPAddInAuthenticationOptions
			{
				ClientId = ConfigurationManager.AppSettings["ClientId"],
				Provider = new SPAddinAuthenticationProvider
				{
					OnAuthenticated = context =>
					{
						var ctx = context.User.Context;

						ctx.Load(context.User.Groups);
						ctx.ExecuteQuery();

						foreach (Group userGroup in context.User.Groups)
						{
							context.Identity.AddClaim(new Claim(ClaimTypes.Role, userGroup.Title));
						}

						return Task.FromResult<object>(null);
					}
				}
			});
		}
	}
}