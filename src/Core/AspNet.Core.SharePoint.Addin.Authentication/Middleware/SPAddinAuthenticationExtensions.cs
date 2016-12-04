using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace AspNet.Core.SharePoint.Addin.Authentication.Middleware
{
	public static class SPAddinAuthenticationExtensions
	{
		public static IApplicationBuilder UseSPAddinAuthentication(this IApplicationBuilder app, SPAddinAuthenticationOptions options)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			return app.UseMiddleware<SPAddinAuthenticationMiddleware>(Options.Create(options));
		}
	}
}
