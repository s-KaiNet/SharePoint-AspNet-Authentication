using System.Security.Claims;
using System.Web.Http;
using AspNet.Owin.SharePoint.Addin.Context;

namespace SPAddinOwin.Sample.UrlPath.Controllers
{
	[Authorize]
	public class WebApiController : ApiController
	{
		public string Get()
		{
			var spContext = SPContextProvider.Get(User as ClaimsPrincipal);
			using (var ctx = spContext.CreateUserClientContextForSPHost())
			{
				ctx.Load(ctx.Web);
				ctx.ExecuteQuery();

				return ctx.Web.Title;
			}
		}
	}
}
