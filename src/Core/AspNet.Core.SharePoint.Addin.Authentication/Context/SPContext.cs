using System;
using System.Security.Claims;
using AspNet.Core.SharePoint.Addin.Authentication.Caching;
using AspNet.Core.SharePoint.Addin.Authentication.Common;
using Microsoft.SharePoint.Client;

namespace AspNet.Core.SharePoint.Addin.Authentication.Context
{
	public abstract class SPContext
	{
		public static ITokenCache Cache;

		protected readonly ClaimsPrincipal _claimsPrincipal;

		protected string RefreshToken
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.RefreshToken))
				{
					throw new Exception("Unable to find Refresh Token under current user's claims");
				}

				return _claimsPrincipal.FindFirst(SPAddinClaimTypes.RefreshToken).Value;
			}
		}

		protected string UserKey
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.CacheKey))
				{
					throw new Exception("Unable to find User Hash Key under current user's claims");
				}

				return _claimsPrincipal.FindFirst(SPAddinClaimTypes.CacheKey).Value;
			}
		}

		protected string Realm
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.Realm))
				{
					throw new Exception("Unable to find Realm under current user's claims");
				}

				return _claimsPrincipal.FindFirst(SPAddinClaimTypes.Realm).Value;
			}
		}

		protected string TargetPrincipalName
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.TargetPrincipalName))
				{
					throw new Exception("Unable to find TargetPrincipalName under current user's claims");
				}

				return _claimsPrincipal.FindFirst(SPAddinClaimTypes.TargetPrincipalName).Value;
			}
		}

		protected Uri SPHostUrl
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.SPHostUrl))
				{
					throw new Exception("Unable to find SPHostUrl under current user's claims");
				}

				return new Uri(_claimsPrincipal.FindFirst(SPAddinClaimTypes.SPHostUrl).Value);
			}
		}

		protected Uri SPAppWebUrl
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == SPAddinClaimTypes.SPAppWebUrl))
				{
					throw new Exception("Unable to find SPAppWebUrl under current user's claims");
				}

				return new Uri(_claimsPrincipal.FindFirst(SPAddinClaimTypes.SPAppWebUrl).Value);
			}
		}

		static SPContext()
		{
			Cache = new DefaultTokenCache();
		}

		protected SPContext(ClaimsPrincipal claimsPrincipal)
		{
			_claimsPrincipal = claimsPrincipal;
		}

		protected ClientContext GetUserClientContext(Uri host)
		{
			var accessToken = Cache.Get(GetUserCacheKey(host.Authority));
			if (accessToken == null)
			{
				accessToken = CreateUserAccessToken(host);
				Cache.Insert(accessToken, GetUserCacheKey(host.Authority));
			}

			return TokenHelper.GetClientContextWithAccessToken(host.AbsoluteUri, accessToken.Value);
		}

		protected ClientContext GetAppOnlyClientContext(Uri host)
		{
			var accessToken = Cache.Get(GetAppOnlyCacheKey(host.Authority));
			if (accessToken == null)
			{
				accessToken = CreateAppOnlyAccessToken(host);
				Cache.Insert(accessToken, GetAppOnlyCacheKey(host.Authority));
			}

			return TokenHelper.GetClientContextWithAccessToken(host.AbsoluteUri, accessToken.Value);
		}

		public ClientContext CreateUserClientContextForSPHost()
		{
			return GetUserClientContext(SPHostUrl);
		}

		public ClientContext CreateUserClientContextForSPAppWeb()
		{
			return GetUserClientContext(SPAppWebUrl);
		}

		public ClientContext CreateAppOnlyClientContextForSPHost()
		{
			return GetAppOnlyClientContext(SPHostUrl);
		}

		public ClientContext CreateAppOnlyClientContextForSPAppWeb()
		{
			return GetAppOnlyClientContext(SPAppWebUrl);
		}

		protected abstract AccessToken CreateAppOnlyAccessToken(Uri host);
		protected abstract AccessToken CreateUserAccessToken(Uri host);

		protected string GetUserCacheKey(string host)
		{
			return $"{UserKey}_{host}";
		}

		protected string GetAppOnlyCacheKey(string host)
		{
			return $"{Realm}_{host}";
		}

		protected bool IsAccessTokenValid(AccessToken token)
		{
			return !string.IsNullOrEmpty(token?.Value) && token.ExpiredOn > DateTime.UtcNow;
		}
	}
}
