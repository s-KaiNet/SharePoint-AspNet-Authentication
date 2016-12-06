using AspNet.Owin.SharePoint.Addin.Authentication.Common;
using AspNet.Owin.SharePoint.Addin.Authentication.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddInAuthenticationOptions : AuthenticationOptions
	{
		public string ClientId { get; set; }

		public string SignInAsAuthenticationType { get; set; }

		public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

		public PathString CallbackPath { get; set; }

		public ISPAddinAuthenticationProvider Provider { get; set; }

		public SPAddInAuthenticationOptions() : base(SPAddinAuthenticationDefaults.AuthenticationType)
		{
			Description.Caption = SPAddinAuthenticationDefaults.AuthenticationType;
			CallbackPath = new PathString("/signin-spaddin/");
			AuthenticationMode = AuthenticationMode.Passive;
		}
	}
}
