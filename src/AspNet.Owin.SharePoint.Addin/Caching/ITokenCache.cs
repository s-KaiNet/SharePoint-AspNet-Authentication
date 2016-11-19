using AspNet.Owin.SharePoint.Addin.Common;

namespace AspNet.Owin.SharePoint.Addin.Caching
{
	public interface ITokenCache
	{
		void Insert(AccessToken token, string key);
		void Remove(string key);
		AccessToken Get(string key);
		bool IsAccessTokenValid(AccessToken token);
	}
}
