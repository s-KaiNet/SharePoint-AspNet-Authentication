using AspNet.Core.SharePoint.Addin.Authentication.Common;

namespace AspNet.Core.SharePoint.Addin.Authentication.Caching
{
	public interface ITokenCache
	{
		void Insert(AccessToken token, string key);
		void Remove(string key);
		AccessToken Get(string key);
		bool IsAccessTokenValid(AccessToken token);
	}
}
