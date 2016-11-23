using System.Web;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Authentication.Middleware;
using Microsoft.Owin.Security;
using SPAddinOwin.Sample.UrlPath.Common;

namespace SPAddinOwin.Sample.UrlPath.Controllers
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

			return new ChallengeResult(Constants.DefaultAuthenticationType, host.Url, redirectUrl);
		}

		//GET: Login
		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			var shortHandUrl = RouteData.Values["shortUrl"].ToString();

			var navigationManager = new NavigationManager(HttpContext.GetOwinContext());

			var host = navigationManager.GetHostByShortHandUrl(shortHandUrl);
			if (host == null)
			{
				return HttpNotFound();
			}

			return new ChallengeResult(Constants.DefaultAuthenticationType, host.Url, returnUrl);
		}

		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string hostUrl, string redirectUri)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				SPHostUrl = hostUrl;
			}

			private string LoginProvider { get; }
			private string RedirectUri { get; }
			private string SPHostUrl { get; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				properties.Dictionary["SPHostUrl"] = SPHostUrl;

				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
	}
}