using System;
using System.Security.Claims;
using AspNet.Owin.SharePoint.Addin.Authentication.Caching;
using AspNet.Owin.SharePoint.Addin.Authentication.Common;
using Microsoft.SharePoint.Client;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Context
{
	//TODO - on prem caching? verifty Realm for on prem
	public abstract class SPContext
	{
		public static ITokenCache Cache;

		protected readonly ClaimsPrincipal _claimsPrincipal;

		protected string RefreshToken
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.RefreshToken))
				{
					throw new Exception("Unable to find Refresh Token under current user's claims");
				}

				return _claimsPrincipal.FindFirst(CustomClaimTypes.RefreshToken).Value;
			}
		}

		protected string UserKey
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.CacheKey))
				{
					throw new Exception("Unable to find User Hash Key under current user's claims");
				}

				return _claimsPrincipal.FindFirst(CustomClaimTypes.CacheKey).Value;
			}
		}

		protected string Realm
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.Realm))
				{
					throw new Exception("Unable to find Realm under current user's claims");
				}

				return _claimsPrincipal.FindFirst(CustomClaimTypes.Realm).Value;
			}
		}

		protected string TargetPrincipalName
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.TargetPrincipalName))
				{
					throw new Exception("Unable to find TargetPrincipalName under current user's claims");
				}

				return _claimsPrincipal.FindFirst(CustomClaimTypes.TargetPrincipalName).Value;
			}
		}

		protected Uri SPHostUrl
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.SPHostUrl))
				{
					throw new Exception("Unable to find SPHostUrl under current user's claims");
				}

				return new Uri(_claimsPrincipal.FindFirst(CustomClaimTypes.SPHostUrl).Value);
			}
		}

		protected Uri SPAppWebUrl
		{
			get
			{
				if (!_claimsPrincipal.HasClaim(c => c.Type == CustomClaimTypes.SPAppWebUrl))
				{
					throw new Exception("Unable to find SPAppWebUrl under current user's claims");
				}

				return new Uri(_claimsPrincipal.FindFirst(CustomClaimTypes.SPAppWebUrl).Value);
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
