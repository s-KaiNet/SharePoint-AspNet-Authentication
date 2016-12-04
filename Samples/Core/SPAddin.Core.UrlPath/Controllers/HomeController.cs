using AspNet.Core.SharePoint.Addin.Authentication.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SPAddin.Core.UrlPath.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public IActionResult Index(string shortUrl)
		{
			var spcontext = SPContextProvider.Get(User);

			using (var clientContext = spcontext.CreateUserClientContextForSPHost())
			{
				clientContext.Load(clientContext.Web.CurrentUser);
				clientContext.Load(clientContext.Web);
				clientContext.ExecuteQuery();

				ViewBag.User = clientContext.Web.CurrentUser.LoginName;
				ViewBag.Host = clientContext.Web.Title;
			}
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
