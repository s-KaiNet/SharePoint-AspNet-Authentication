using System.Security.Claims;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Context
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
