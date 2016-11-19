using System.Threading.Tasks;

namespace AspNet.Owin.SharePoint.Addin.Provider
{
	public interface ISPAddinAuthenticationProvider
	{
		Task Authenticated(SPAddinAuthenticatedContext context);
	}
}
