using System.Web.Mvc;
using System.Web.Routing;

namespace SPAddinOwin.Sample.QueryString
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Auth",
				url: "Auth",
				defaults: new { controller = "Auth", action = "AppRedirect" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}",
				defaults: new { controller = "Home", action = "Index" }
			);
		}
	}
}
