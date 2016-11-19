using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;
using Microsoft.SharePoint.Client;

namespace AspNet.Owin.SharePoint.Addin.Provider
{
	public class SPAddinAuthenticatedContext : BaseContext
	{
		public User User { get; private set; }

		public ClaimsIdentity Identity { get; private set; }

		public SPAddinAuthenticatedContext(IOwinContext context, User user, ClaimsIdentity identity) : base(context)
		{
			User = user;
			Identity = identity;
		}
	}
}
