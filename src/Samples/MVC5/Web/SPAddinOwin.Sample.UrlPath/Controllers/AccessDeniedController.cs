using System.Web.Mvc;

namespace SPAddinOwin.Sample.UrlPath.Controllers
{
	public class AccessDeniedController : Controller
	{
		// GET: AccessDenied
		public ActionResult Index(string roles)
		{
			ViewBag.Roles = roles;

			return View();
		}
	}
}