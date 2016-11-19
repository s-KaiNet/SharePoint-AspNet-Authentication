using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SPAddinOwin.Sample.QueryString.Common;

namespace SPAddinOwin.Sample.QueryString
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public MvcApplication()
		{
			EndRequest += (sender, args) =>
			{
				if (Context.Response.StatusCode == 418)
				{
					Context.Response.StatusCode = 401;
					Context.Response.SubStatusCode = 2;
				}
			};
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			GlobalFilters.Filters.Add(new HostUrlActionFilter(), 0);
		}
	}
}
