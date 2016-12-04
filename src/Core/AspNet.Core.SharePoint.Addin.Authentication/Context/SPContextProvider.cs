using System.Security.Claims;
using AspNet.Core.SharePoint.Addin.Authentication.Common;

namespace AspNet.Core.SharePoint.Addin.Authentication.Context
{
	public static class SPContextProvider
	{
		public static SPContext Get(ClaimsPrincipal claimsPrincipal)
		{
			if (!TokenHelper.IsHighTrustApp())
			{
				return new AcsContext(claimsPrincipal);
			}

			return new HighTrustContext(claimsPrincipal);
		}
	}
}
