using System;
using System.Security.Claims;
using AspNet.Core.SharePoint.Addin.Authentication.Common;

namespace AspNet.Core.SharePoint.Addin.Authentication.Context
{
	public class HighTrustContext : SPContext
	{
		private readonly string _userId;

		public HighTrustContext(ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal)
		{
			_userId = claimsPrincipal.FindFirst(c => c.Type.Equals(SPAddinClaimTypes.ADUserId)).Value;
		}

		protected override AccessToken CreateAppOnlyAccessToken(Uri host)
		{
			var s2sToken = TokenHelper.GetS2SAccessToken(host, null);

			return new AccessToken
			{
				Value = s2sToken,
				ExpiredOn = DateTime.UtcNow.Add(TokenHelper.HighTrustAccessTokenLifetime).AddMinutes(-5)
			};
		}

		protected override AccessToken CreateUserAccessToken(Uri host)
		{
			var s2sToken = TokenHelper.GetS2SAccessToken(host, _userId);

			return new AccessToken
			{
				Value = s2sToken,
				ExpiredOn = DateTime.UtcNow.Add(TokenHelper.HighTrustAccessTokenLifetime).AddMinutes(-5)
			};
		}
	}
}
