namespace AspNet.Core.SharePoint.Addin.Authentication.Common
{
	public interface ILowTrustSettings : IAuthSettings
	{
		string ClientSecret { get; set; }
		string Realm { get; set; }
		string HostedAppHostNameOverride { get; set; }
		string HostedAppHostName { get; set; }
		string SecondaryClientSecret { get; set; }
	}
}
