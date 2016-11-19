using System.Web.Mvc;

namespace SPAddinOwin.Sample.QueryString.Common
{
	public class HostUrlActionFilter : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.Controller.ViewBag.HostUrl = filterContext.HttpContext.Request.QueryString["h"];
		}
	}
}