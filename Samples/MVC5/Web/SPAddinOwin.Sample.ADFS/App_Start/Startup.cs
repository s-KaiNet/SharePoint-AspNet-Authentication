using System.Configuration;
using AspNet.Owin.SharePoint.Addin.Authentication.Middleware;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using SPAddinOwin.Data.DB;
using SPAddinOwin.Sample.ADFS;
using SPAddinOwin.Sample.ADFS.Common;

[assembly: OwinStartup(typeof(Startup))]

namespace SPAddinOwin.Sample.ADFS
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

			app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
			{
				MetadataAddress = ConfigurationManager.AppSettings["MetadataAddress"],
				BackchannelCertificateValidator = new PassThroughCertificateValidator(),
				Wtrealm = ConfigurationManager.AppSettings["Wtrealm"],
				AuthenticationMode = AuthenticationMode.Passive
			});

			app.UseSPAddinAuthentication(new SPAddInAuthenticationOptions
			{
				ClientId = ConfigurationManager.AppSettings["ClientId"]
			});
		}
	}
}