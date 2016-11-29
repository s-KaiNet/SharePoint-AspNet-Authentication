using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Owin.SharePoint.Addin.Authentication.Common;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;

namespace SPAddinOwin.Sample.ADFS.Common
{
	public class AdddInCookieAuthenticationProvider : ICookieAuthenticationProvider
	{
		public Task ValidateIdentity(CookieValidateIdentityContext context)
		{
			if (context.Identity.IsAuthenticated)
			{
				var shortHandUrl = context.Request.Path.ToString()
					.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
					.ToList()
					.FirstOrDefault();

				if (shortHandUrl != null && (shortHandUrl.Equals("auth", StringComparison.OrdinalIgnoreCase) ||
					shortHandUrl.StartsWith("signin") || context.Request.Path.Value.Contains(context.Options.LoginPath.Value) ||
					!context.Identity.HasClaim(c => c.Type == SPAddinClaimTypes.ShortHandUrl)))
				{
					return Task.FromResult<object>(null);
				}

				var shortHandUrlClaim = context.Identity.FindFirst(SPAddinClaimTypes.ShortHandUrl).Value;

				if (!shortHandUrlClaim.Equals(shortHandUrl, StringComparison.OrdinalIgnoreCase))
				{
					context.RejectIdentity();
				}
			}
			return Task.FromResult<object>(null);
		}

		public void ResponseSignIn(CookieResponseSignInContext context)
		{
			if (context.Identity.HasClaim(c => c.Type == SPAddinClaimTypes.SPAddinAuthentication) && 
				!context.Identity.HasClaim(c => c.Type == SPAddinClaimTypes.ShortHandUrl))
			{
				var hostUrl = context.Identity.FindFirst(SPAddinClaimTypes.SPHostUrl).Value;

				var navigationManager = new NavigationManager(context.OwinContext);

				var host = navigationManager.EnsureHostByUrl(hostUrl);

				context.Identity.AddClaim(new Claim(SPAddinClaimTypes.ShortHandUrl, host.ShortHandUrl));
			}
		}

		public void ApplyRedirect(CookieApplyRedirectContext context)
		{
			var shortHandUrl = context.Request.Path.ToString()
							.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
							.ToList()
							.FirstOrDefault();

			if (shortHandUrl == null)
			{
				throw new Exception("Unable to determine host url");
			}
			var queryString = new Uri(context.RedirectUri).ParseQueryString();
			var returnUrl = queryString[context.Options.ReturnUrlParameter];
			var authRedirect = new UriBuilder(context.Request.Uri.GetLeftPart(UriPartial.Path))
			{
				Path = shortHandUrl + context.Options.LoginPath
			};
			var redirectUrl = WebUtilities.AddQueryString(authRedirect.ToString(), context.Options.ReturnUrlParameter, returnUrl);

			context.Response.Redirect(redirectUrl);
		}

		public void ResponseSignOut(CookieResponseSignOutContext context)
		{
		}

		public void Exception(CookieExceptionContext context)
		{
		}

		public void ResponseSignedIn(CookieResponseSignedInContext context)
		{
			if (!context.Identity.HasClaim(c => c.Type == SPAddinClaimTypes.SPAddinAuthentication))
			{
				var shortHandUrl = context.Properties.RedirectUri.Replace("/", string.Empty);

				var navigationManager = new NavigationManager(context.OwinContext);

				var host = navigationManager.GetHostByShortHandUrl(shortHandUrl);
				if (host == null)
				{
					throw new Exception("Unable to find host url");
				}

				var authRedirect = new UriBuilder(context.Request.Uri.GetLeftPart(UriPartial.Path))
				{
					Path = shortHandUrl + "/Auth/AddinLogin"
				};
				var redirectUrl = WebUtilities.AddQueryString(authRedirect.ToString(), context.Options.ReturnUrlParameter, context.Properties.RedirectUri);
				redirectUrl = WebUtilities.AddQueryString(redirectUrl, "hostUrl", host.Url);

				context.Response.Redirect(redirectUrl);
			}
		}
	}
}