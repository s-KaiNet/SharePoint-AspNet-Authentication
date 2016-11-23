using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SPAddinOwin.Sample.UrlPath
{
	public class MvcApplication : HttpApplication
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
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}
