namespace AspNet.Core.SharePoint.Addin.Authentication.Common
{
	public interface IHighTrustSettings : IAuthSettings
	{
		string IssuerId { get; set; }
		string ClientSigningCertificatePath { get; set; }
		string ClientSigningCertificatePassword { get; set; }
	}
}
