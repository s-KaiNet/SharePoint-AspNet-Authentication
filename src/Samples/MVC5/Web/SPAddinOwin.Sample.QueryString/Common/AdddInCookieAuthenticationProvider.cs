using System;
using System.Threading.Tasks;
using AspNet.Owin.SharePoint.Addin.Common;
using Microsoft.Owin.Security.Cookies;

namespace SPAddinOwin.Sample.QueryString.Common
{
	public class AdddInCookieAuthenticationProvider : ICookieAuthenticationProvider
	{
		public Task ValidateIdentity(CookieValidateIdentityContext context)
		{
			if (context.Identity.IsAuthenticated)
			{
				var queryStringHostUrl = context.Request.Query["h"];

				if (context.Request.Path.Value.Contains("Auth") ||
					context.Request.Path.Value.StartsWith("signin") || context.Request.Path.Value.Contains(context.Options.LoginPath.Value))
				{
					return Task.FromResult<object>(null);
				}

				if (string.IsNullOrEmpty(queryStringHostUrl))
				{
					throw new Exception("Unable to determine host url");
				}

				var hostUrl = context.Identity.FindFirst(CustomClaimTypes.SPHostUrl).Value;

				if (!hostUrl.Equals(queryStringHostUrl, StringComparison.OrdinalIgnoreCase))
				{
					context.RejectIdentity();
				}
			}
			return Task.FromResult<object>(null);
		}

		public void ResponseSignIn(CookieResponseSignInContext context)
		{
		}

		public void ApplyRedirect(CookieApplyRedirectContext context)
		{
			context.Response.Redirect(context.RedirectUri);
		}

		public void ResponseSignOut(CookieResponseSignOutContext context)
		{
		}

		public void Exception(CookieExceptionContext context)
		{
		}

		public void ResponseSignedIn(CookieResponseSignedInContext context)
		{
		}
	}
}