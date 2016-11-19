using Owin;

namespace AspNet.Owin.SharePoint.Addin.Auth
{
	public static class SPAddInAuthenticationExtensions
	{
		public static IAppBuilder UseSPAddinAuthentication(this IAppBuilder app, SPAddInAuthenticationOptions options)
		{
			return app.Use(typeof(SPAddInAuthenticationMiddleware), app, options);
		}
	}
}
