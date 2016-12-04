using AspNet.Core.SharePoint.Addin.Authentication.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using SPAddin.Core.UrlPath.Common;
using SPAddin.Core.UrlPath.DB;

namespace SPAddin.Core.UrlPath.Controllers
{
	[AllowAnonymous]
	public class AuthController : Controller
	{
		private readonly AddInContext _context;

		public AuthController(AddInContext context)
		{
			_context = context;
		}

		// POST: AppRedirect
		[HttpPost]
		public ActionResult AppRedirect(string hostUrl)
		{
			var navigationManager = new NavigationManager(_context);
			var host = navigationManager.EnsureHostByUrl(hostUrl);

			var redirectUrl = $"/{host.ShortHandUrl}";

			if (User.Identity.IsAuthenticated)
			{
				return Redirect(redirectUrl);
			}

			var properties = new AuthenticationProperties
			{
				RedirectUri = redirectUrl
			};

			properties.Items.Add("SPHostUrl", host.Url);

			return Challenge(properties, SPAddinAuthenticationDefaults.AuthenticationType);
		}

		//GET: Login
		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			var shortHandUrl = RouteData.Values["shortUrl"].ToString();

			var navigationManager = new NavigationManager(_context);

			var host = navigationManager.GetHostByShortHandUrl(shortHandUrl);
			if (host == null)
			{
				return NotFound();
			}

			var properties = new AuthenticationProperties
			{
				RedirectUri = returnUrl
			};

			properties.Items.Add("SPHostUrl", host.Url);

			return Challenge(properties, SPAddinAuthenticationDefaults.AuthenticationType);
		}

	}
}