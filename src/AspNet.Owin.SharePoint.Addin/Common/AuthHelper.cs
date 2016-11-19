using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.Owin;
using Microsoft.SharePoint.Client;
using FormCollection = Microsoft.Owin.FormCollection;

namespace AspNet.Owin.SharePoint.Addin.Common
{
	internal static class AuthHelper
	{
		public static string GetS2SAccessToken(Uri applicationUri, string userId)
		{
			var realm = TokenHelper.GetRealmFromTargetUrl(applicationUri);

			JsonWebTokenClaim[] claims = null;

			if (userId != null)
			{
				claims = new[]
				{
					new JsonWebTokenClaim(JsonWebTokenConstants.ReservedClaims.NameIdentifier, userId.ToLower()),
					new JsonWebTokenClaim("nii", "urn:office:idp:activedirectory")
				};
			}

			return GetS2SAccessTokenWithClaims(applicationUri.Authority, realm, claims);
		}

		public static string GetAppContextTokenRequestUrl(string contextUrl, string redirectUri)
		{
			return TokenHelper.GetAppContextTokenRequestUrl(contextUrl, redirectUri);
		}

		public static ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
		{
			return TokenHelper.GetClientContextWithAccessToken(targetUrl, accessToken);
		}

		public static string EnsureTrailingSlash(string url)
		{
			return TokenHelper.EnsureTrailingSlash(url);
		}

		public static SharePointContextToken ReadAndValidateContextToken(string contextTokenString, string appHostName)
		{
			return TokenHelper.ReadAndValidateContextToken(contextTokenString, appHostName);
		}

		public static string GetRealmFromTargetUrl(Uri targetApplicationUri)
		{
			return TokenHelper.GetRealmFromTargetUrl(targetApplicationUri);
		}

		public static bool IsHighTrustApp()
		{
			return TokenHelper.IsHighTrustApp();
		}

		public static string GetAcsAccessToken(string refreshToken, string targetPrincipalName, string targetHost, string targetRealm)
		{
			return TokenHelper.GetAccessToken(refreshToken, targetPrincipalName, targetHost, targetRealm).AccessToken;
		}

		public static string GetContextTokenFromRequest(IOwinRequest request)
		{
			using (var reader = new StreamReader(request.Body, Encoding.UTF8, true))
			{
				var formData = GetForm(reader.ReadToEnd());

				string[] paramNames = { "AppContext", "AppContextToken", "AccessToken", "SPAppToken" };

				foreach (string paramName in paramNames)
				{
					if (!string.IsNullOrEmpty(formData[paramName]))
					{
						return formData[paramName];
					}
					if (!string.IsNullOrEmpty(request.Query[paramName]))
					{
						return request.Query[paramName];
					}
				}
			}
			return null;
		}

		public static string GetWindowsUserId(IOwinContext context)
		{
			var httpRequest = ((System.Web.HttpContextBase)context.Environment["System.Web.HttpContextBase"]).Request;
			
			return httpRequest.LogonUserIdentity.User.Value;
		}

		public static WindowsIdentity GetWindowsUser(IOwinContext context)
		{
			var httpRequest = ((System.Web.HttpContextBase)context.Environment["System.Web.HttpContextBase"]).Request;

			return httpRequest.LogonUserIdentity;
		}

		private static string GetS2SAccessTokenWithClaims(string targetApplicationHostName, string targetRealm, IEnumerable<JsonWebTokenClaim> claims)
		{
			var method = typeof(TokenHelper).GetMethod(nameof(GetS2SAccessTokenWithClaims), BindingFlags.Static | BindingFlags.NonPublic);

			return (string)method.Invoke(null, new object[] { targetApplicationHostName, targetRealm, claims });
		}

		private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) =>
		{
			var dictionary = (IDictionary<string, List<String>>)state;

			List<string> existing;
			if (!dictionary.TryGetValue(name, out existing))
			{
				dictionary.Add(name, new List<string>(1) { value });
			}
			else
			{
				existing.Add(value);
			}
		};


		private static IFormCollection GetForm(string text)
		{
			IDictionary<string, string[]> form = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
			var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			ParseDelimited(text, new[] { '&' }, AppendItemCallback, accumulator);
			foreach (var kv in accumulator)
			{
				form.Add(kv.Key, kv.Value.ToArray());
			}
			return new FormCollection(form);
		}

		private static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
		{
			int textLength = text.Length;
			int equalIndex = text.IndexOf('=');
			if (equalIndex == -1)
			{
				equalIndex = textLength;
			}
			int scanIndex = 0;
			while (scanIndex < textLength)
			{
				int delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
				if (delimiterIndex == -1)
				{
					delimiterIndex = textLength;
				}
				if (equalIndex < delimiterIndex)
				{
					while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
					{
						++scanIndex;
					}
					string name = text.Substring(scanIndex, equalIndex - scanIndex);
					string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
					callback(
						Uri.UnescapeDataString(name.Replace('+', ' ')),
						Uri.UnescapeDataString(value.Replace('+', ' ')),
						state);
					equalIndex = text.IndexOf('=', delimiterIndex);
					if (equalIndex == -1)
					{
						equalIndex = textLength;
					}
				}
				scanIndex = delimiterIndex + 1;
			}
		}
	}
}
