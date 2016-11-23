using System.Security.Claims;
using System.Web.Mvc;
using SPAddinOwin.Sample.UrlPath.Common;

namespace SPAddinOwin.Sample.UrlPath.Base
{
	public class SPController : Controller
	{
		private SharePointUserPrincipal _spPrincipal;

		public SharePointUserPrincipal SPUserPrincipal => _spPrincipal ?? (_spPrincipal = new SharePointUserPrincipal(User as ClaimsPrincipal));
	}
}