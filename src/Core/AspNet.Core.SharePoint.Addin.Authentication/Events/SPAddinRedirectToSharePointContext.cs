using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace AspNet.Core.SharePoint.Addin.Authentication.Events
{
    public class SPAddinRedirectToSharePointContext : BaseContext
	{
		public string RedirectUri { get; private set; }

		public AuthenticationProperties Properties { get; private set; }

		public SPAddinRedirectToSharePointContext(HttpContext context, AuthenticationProperties properties, string redirectUri) : base(context)
		{
			RedirectUri = redirectUri;
			Properties = properties;
		}
	}
}
