using System;
using System.Web;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Authentication.Middleware;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.WsFederation;
using SPAddinOwin.Sample.ADFS.Common;

namespace SPAddinOwin.Sample.ADFS.Controllers
{
	[AllowAnonymous]
	public class AuthController : Controller
	{
		// POST: AppRedirect
		[HttpPost]
		public ActionResult AppRedirect(string hostUrl)
		{
			var navigationManager = new NavigationManager(HttpContext.GetOwinContext());
			var host = navigationManager.EnsureHostByUrl(hostUrl);

			var redirectUrl = $"/{host.ShortHandUrl}";

			if (User.Identity.IsAuthenticated)
			{
				return Redirect(redirectUrl);
			}

			return new ChallengeResult(WsFederationAuthenticationDefaults.AuthenticationType, null, redirectUrl);
		}

		// GET: Login
		public ActionResult Login(string returnUrl)
		{
			var shortHandUrl = RouteData.Values["shortUrl"].ToString();

			var navigationManager = new NavigationManager(HttpContext.GetOwinContext());

			var host = navigationManager.GetHostByShortHandUrl(shortHandUrl);
			if (host == null)
			{
				throw new Exception("Unable to find host url");
			}

			return new ChallengeResult(WsFederationAuthenticationDefaults.AuthenticationType, null, returnUrl);
		}

		public ActionResult AddinLogin(string hostUrl, string returnUrl)
		{
			return new ChallengeResult(SPAddinAuthenticationDefaults.AuthenticationType, hostUrl, returnUrl);
		}

		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string hostUrl, string redirectUri)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				HostUrl = hostUrl;
			}

			private string LoginProvider { get; }
			private string RedirectUri { get; }
			private string HostUrl { get; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (HostUrl != null)
				{
					properties.Dictionary["SPHostUrl"] = HostUrl;
				}

				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
	}
}