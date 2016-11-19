using System.Web.Mvc;
using System.Web.Routing;

namespace SPAddinOwin.Sample.UrlPath.Filters
{
	public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.Result is HttpUnauthorizedResult)
			{
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
					new { shortUrl = filterContext.RouteData.Values["shortUrl"], controller = "AccessDenied", roles = Roles}));
			}
		}
	}
}