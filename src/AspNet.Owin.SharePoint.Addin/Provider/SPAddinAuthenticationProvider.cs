using System;
using System.Threading.Tasks;

namespace AspNet.Owin.SharePoint.Addin.Provider
{
	public class SPAddinAuthenticationProvider : ISPAddinAuthenticationProvider
	{
		public Func<SPAddinAuthenticatedContext, Task> OnAuthenticated { get; set; }

		public SPAddinAuthenticationProvider()
		{
			OnAuthenticated = context => Task.FromResult<object>(null);
		}

		public Task Authenticated(SPAddinAuthenticatedContext context)
		{
			return OnAuthenticated(context);
		}
	}
}
