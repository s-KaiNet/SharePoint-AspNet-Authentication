using System.Security.Claims;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Context;

namespace SPAddinOwin.Sample.QueryString.Controllers
{
	[Authorize]
	public class HomeController : Controller
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
	}
}