using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Core.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddinAuthenticationMiddleware : AuthenticationMiddleware<SPAddinAuthenticationOptions>
	{
		public SPAddinAuthenticationMiddleware(
			RequestDelegate next,
			IDataProtectionProvider dataProtectionProvider,
			ILoggerFactory loggerFactory,
			UrlEncoder encoder,
			IOptions<SPAddinAuthenticationOptions> options)
			: base(next, options, loggerFactory, encoder)
		{
			if (Options.StateDataFormat == null)
			{
				var dataProtector = dataProtectionProvider.CreateProtector(
					GetType().FullName, Options.AuthenticationScheme, "v1");
				Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
			}
		}

		protected override AuthenticationHandler<SPAddinAuthenticationOptions> CreateHandler()
		{
			return new SPAddinAuthenticationHandler(Options.AuthSettings);
		}
	}
}
