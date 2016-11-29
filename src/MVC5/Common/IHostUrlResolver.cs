using Microsoft.Owin;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Common
{
	public interface IHostUrlResolver
	{
		string ResolveHostUrl(IOwinContext context);
	}
}
