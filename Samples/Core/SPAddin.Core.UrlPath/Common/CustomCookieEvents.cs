using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Core.SharePoint.Addin.Authentication.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using SPAddin.Core.UrlPath.DB;

namespace SPAddin.Core.UrlPath.Common
{
	public class CustomCookieEvents : CookieAuthenticationEvents
	{
		private readonly IServiceProvider _serviceProvider;

		public CustomCookieEvents(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
		{
			if (context.Principal.Identity.IsAuthenticated)
			{
				var shortHandUrl = context.Request.Path.ToString()
					.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
					.ToList()
					.FirstOrDefault();

				if (shortHandUrl != null && (shortHandUrl.Equals("auth", StringComparison.OrdinalIgnoreCase) ||
					shortHandUrl.StartsWith("signin") || context.Request.Path.Value.Contains(context.Options.LoginPath.Value)))
				{
					return Task.FromResult<object>(null);
				}
				
				var shortHandUrlClaim = context.Principal.FindFirst(SPAddinClaimTypes.ShortHandUrl).Value;

				if (!shortHandUrlClaim.Equals(shortHandUrl, StringComparison.OrdinalIgnoreCase))
				{
					context.RejectPrincipal();
				}
			}
			return Task.FromResult<object>(null);
		}

		public override Task SigningIn(CookieSigningInContext context)
		{
			using (var dbContext = _serviceProvider.GetService<AddInContext>())
			{
				var hostUrl = ((ClaimsIdentity)context.Principal.Identity).FindFirst(SPAddinClaimTypes.SPHostUrl).Value;

				var navigationManager = new NavigationManager(dbContext);

				var host = navigationManager.EnsureHostByUrl(hostUrl);

				((ClaimsIdentity)context.Principal.Identity).AddClaim(new Claim(SPAddinClaimTypes.ShortHandUrl, host.ShortHandUrl));

				return Task.FromResult<object>(null);
			}
		}

		public override Task RedirectToLogin(CookieRedirectContext context)
		{
			var shortHandUrl = context.Request.Path.ToString()
							.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
							.ToList()
							.FirstOrDefault();

			if (shortHandUrl == null)
			{
				throw new Exception("Unable to determine host url");
			}
			var queryString = QueryHelpers.ParseQuery(new Uri(context.RedirectUri).Query);
			var returnUrl = queryString[context.Options.ReturnUrlParameter];
			var authRedirect = new UriBuilder(context.Request.Path)
			{
				Path = shortHandUrl + context.Options.LoginPath
			};
			var redirectUrl = QueryHelpers.AddQueryString(authRedirect.ToString(), context.Options.ReturnUrlParameter, returnUrl);

			context.Response.Redirect(redirectUrl);

			return Task.FromResult<object>(null);
		}
	}
}
