using System;
using System.Collections.Generic;
using AspNet.Core.SharePoint.Addin.Authentication.Common;

namespace AspNet.Core.SharePoint.Addin.Authentication.Caching
{
	public class DefaultTokenCache : ITokenCache
	{
		protected static Dictionary<string, AccessToken> _tokens;

		static DefaultTokenCache()
		{
			_tokens = new Dictionary<string, AccessToken>();
		}

		public void Insert(AccessToken token, string key)
		{
			_tokens[key] = token;
		}

		public void Remove(string key)
		{
			if (_tokens.ContainsKey(key))
			{
				_tokens.Remove(key);
			}
		}

		public AccessToken Get(string key)
		{
			if (_tokens.ContainsKey(key) && IsAccessTokenValid(_tokens[key]))
			{
				return _tokens[key];
			}

			Remove(key);

			return null;
		}

		public bool IsAccessTokenValid(AccessToken token)
		{
			return !string.IsNullOrEmpty(token?.Value) && token.ExpiredOn > DateTime.UtcNow;
		}
	}
}
