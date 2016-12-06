using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AspNet.Core.SharePoint.Addin.Authentication.Events
{
	public class SPAddinEvents : RemoteAuthenticationEvents
	{
		public Func<SPAddinRedirectToSharePointContext, Task> OnRedirectToAuthorizationEndpoint { get; set; } = context =>
		{
			context.Response.Redirect(context.RedirectUri);
			return Task.FromResult<object>(null);
		};

		public Func<SPAddinOnAuthenticatedContext, Task> OnAuthenticated { get; set; } = context => Task.FromResult<object>(null);

		public virtual Task RedirectToAuthorizationEndpoint(SPAddinRedirectToSharePointContext context) => OnRedirectToAuthorizationEndpoint(context);

		public virtual Task Authenticated(SPAddinOnAuthenticatedContext context) => OnAuthenticated(context);
	}
}
