using AspNet.Core.SharePoint.Addin.Authentication.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace AspNet.Core.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddinAuthenticationOptions : RemoteAuthenticationOptions
	{
		public IAuthSettings AuthSettings { get; set; }
		public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

		public SPAddinAuthenticationOptions()
		{
			CallbackPath = new PathString("/signin-spaddin/");
			ClaimsIssuer = "SPAddin";
		}
	}
}
