using System;

namespace AspNet.Core.SharePoint.Addin.Authentication.Common
{
	public class AccessToken
	{
		public string Value { get; set; }
		public DateTime ExpiredOn { get; set; }
	}
}
