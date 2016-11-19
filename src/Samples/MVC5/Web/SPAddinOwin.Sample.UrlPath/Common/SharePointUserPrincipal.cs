using System;
using System.Security.Claims;
using AspNet.Owin.SharePoint.Addin.Common;

namespace SPAddinOwin.Sample.UrlPath.Common
{
	public class SharePointUserPrincipal : ClaimsPrincipal
	{
		public SharePointUserPrincipal(ClaimsPrincipal principal)
		: base(principal)
		{
		}

		public string LoginName => FindFirst(ClaimTypes.NameIdentifier).Value;
		public string Name => FindFirst(ClaimTypes.Name).Value;
		public string Email => FindFirst(ClaimTypes.Email).Value;
		public string HashKey => FindFirst(CustomClaimTypes.CacheKey).Value;
		public string RefreshToken => FindFirst(CustomClaimTypes.RefreshToken).Value;
		public Uri SPHostUrl => new Uri(FindFirst(CustomClaimTypes.SPHostUrl).Value);
	}
}
