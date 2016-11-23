using AspNet.Owin.SharePoint.Addin.Authentication.Common;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Caching
{
	public interface ITokenCache
	{
		void Insert(AccessToken token, string key);
		void Remove(string key);
		AccessToken Get(string key);
		bool IsAccessTokenValid(AccessToken token);
	}
}
