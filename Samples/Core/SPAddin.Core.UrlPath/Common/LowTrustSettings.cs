using AspNet.Core.SharePoint.Addin.Authentication.Common;

namespace SPAddin.Core.UrlPath.Common
{
	public class LowTrustSettings : ILowTrustSettings
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Realm { get; set; }
		public string HostedAppHostNameOverride { get; set; }
		public string HostedAppHostName { get; set; }
		public string SecondaryClientSecret { get; set; }
	}
}
