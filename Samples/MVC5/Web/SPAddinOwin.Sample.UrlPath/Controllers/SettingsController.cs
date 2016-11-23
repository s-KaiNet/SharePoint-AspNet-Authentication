using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using SPAddinOwin.Data.DB;
using SPAddinOwin.Data.Entities;
using SPAddinOwin.Sample.UrlPath.Base;
using SPAddinOwin.Sample.UrlPath.Common;

namespace SPAddinOwin.Sample.UrlPath.Controllers
{
	[Authorize]
	public class SettingsController : SPController
	{
		// GET: Settings
		[HttpGet]
		public ActionResult Index()
		{
			var manager = new NavigationManager(HttpContext.GetOwinContext());
			var host = manager.EnsureHostByUrl(SPUserPrincipal.SPHostUrl.AbsoluteUri);

			return View(host);
		}

		// POST: Settings
		[HttpPost]
		public ActionResult Index(Host host)
		{
			var addinDbContext = HttpContext.GetOwinContext().Get<AddInContext>();
			addinDbContext.Hosts.Attach(host);

			var entry = addinDbContext.Entry(host);
			entry.Property(e => e.ShortHandUrl).IsModified = true;
			addinDbContext.SaveChanges();

			HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

			return RedirectToAction("Index", "Home", new { shortUrl = host.ShortHandUrl});
		}
	}
}