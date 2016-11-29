using System.Security.Claims;
using System.Web.Mvc;
using AspNet.Owin.SharePoint.Addin.Authentication.Context;

namespace SPAddinOwin.Sample.ADFS.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var spContext = SPContextProvider.Get(User as ClaimsPrincipal);
			using (var ctx = spContext.CreateUserClientContextForSPHost())
			{
				ctx.Load(ctx.Web.CurrentUser);
				ctx.Load(ctx.Web);
				ctx.ExecuteQuery();

				ViewBag.HostTitle = ctx.Web.Title;
			}

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}