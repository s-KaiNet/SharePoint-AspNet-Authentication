using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.SharePoint.Client;

namespace AspNet.Core.SharePoint.Addin.Authentication.Events
{
    public class SPAddinOnAuthenticatedContext : BaseContext
	{
		public User User { get; private set; }

		public ClaimsIdentity Identity { get; private set; }

		public SPAddinOnAuthenticatedContext(HttpContext context, User user, ClaimsIdentity identity) : base(context)
		{
			User = user;
			Identity = identity;
		}
	}
}
