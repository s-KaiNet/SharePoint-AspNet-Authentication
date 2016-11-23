using System.Threading.Tasks;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Provider
{
	public interface ISPAddinAuthenticationProvider
	{
		Task Authenticated(SPAddinAuthenticatedContext context);
	}
}
