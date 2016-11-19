using System.Web.Mvc;
using SPAddinOwin.Sample.UrlPath.Filters;

namespace SPAddinOwin.Sample.UrlPath.Controllers
{
	[AccessDeniedAuthorize(Roles = "Admins")]
	public class SecretController : Controller
	{
		// GET: Secret
		public ActionResult Index()
		{
			return View();
		}
	}
}