using AspNet.Owin.SharePoint.Addin.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace AspNet.Owin.SharePoint.Addin.Auth
{
	public class SPAddInAuthenticationOptions : AuthenticationOptions
	{
		public string ClientId { get; set; }

		public string SignInAsAuthenticationType { get; set; }

		public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

		public PathString CallbackPath { get; set; }

		public ISPAddinAuthenticationProvider Provider { get; set; }

		public SPAddInAuthenticationOptions() : base(Constants.DefaultAuthenticationType)
		{
			Description.Caption = Constants.DefaultAuthenticationType;
			CallbackPath = new PathString("/signin-spaddin/");
			AuthenticationMode = AuthenticationMode.Passive;
		}
	}
}
