using AspNet.Owin.SharePoint.Addin.Authentication.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddInAuthenticationMiddleware : AuthenticationMiddleware<SPAddInAuthenticationOptions>
	{
		private readonly ILogger _logger;

		public SPAddInAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, SPAddInAuthenticationOptions options) 
			:base(next, options)
		{
			_logger = app.CreateLogger<SPAddInAuthenticationMiddleware>();

			if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
			{
				options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
			}
			if (options.StateDataFormat == null)
			{
				var dataProtector = app.CreateDataProtector(typeof(SPAddInAuthenticationMiddleware).FullName,
					options.AuthenticationType);

				options.StateDataFormat = new PropertiesDataFormat(dataProtector);
			}

			if (options.Provider == null)
			{
				options.Provider = new SPAddinAuthenticationProvider();
			}
		}

		protected override AuthenticationHandler<SPAddInAuthenticationOptions> CreateHandler()
		{
			return new SPAddInAuthenticationHandler(_logger);
		}
	}
}
