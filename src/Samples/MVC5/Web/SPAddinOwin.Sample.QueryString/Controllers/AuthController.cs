using System;
using System.Web;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Authentication.Middleware;
using Microsoft.Owin.Security;

namespace SPAddinOwin.Sample.QueryString.Controllers
{
	[AllowAnonymous]
	public class AuthController : Controller
	{
		// POST: AppRedirect
		[HttpPost]
		public ActionResult AppRedirect(string hostUrl)
		{
			var redirectUrl = $"/?h={hostUrl}";

			if (User.Identity.IsAuthenticated)
			{
				return Redirect(redirectUrl);
			}

			return new ChallengeResult(Constants.DefaultAuthenticationType, hostUrl, redirectUrl);
		}

		//GET: Login
		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			var hostUrl = Request.QueryString["h"];

			if (string.IsNullOrEmpty(hostUrl))
			{
				throw new Exception("Unable to determine host url");
			}

			return new ChallengeResult(Constants.DefaultAuthenticationType, hostUrl, returnUrl);
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