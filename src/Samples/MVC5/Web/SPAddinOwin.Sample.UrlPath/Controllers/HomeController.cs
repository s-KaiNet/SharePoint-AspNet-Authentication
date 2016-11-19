using System.Security.Claims;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Context;
using SPAddinOwin.Sample.UrlPath.Base;

namespace SPAddinOwin.Sample.UrlPath.Controllers
{
	[Authorize]
	public class HomeController : SPController
	{
		// GET: Home
		public ActionResult Index()
		{
			var spContext = SPContextProvider.Get(User as ClaimsPrincipal);
			using (var ctx = spContext.CreateUserClientContextForSPHost())
			{
				ctx.Load(ctx.Web.CurrentUser);
				ctx.ExecuteQuery();
			}
			return View();
		}

		//GET: About
		public ActionResult About()
		{
			return View();
		}

		//GET: Ajax
		public ActionResult Ajax()
		{
			return View();
		}
	}
}