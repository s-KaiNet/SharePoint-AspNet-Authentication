using System;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AspNet.Core.SharePoint.Addin.Authentication.Common;
using AspNet.Core.SharePoint.Addin.Authentication.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using X509SigningCredentials = Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials;

namespace AspNet.Core.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddinAuthenticationHandler : RemoteAuthenticationHandler<SPAddinAuthenticationOptions>
	{

		public SPAddinAuthenticationHandler(IAuthSettings authSettings)
		{
			if (authSettings is ILowTrustSettings)
			{
				var settings = (ILowTrustSettings)authSettings;

				TokenHelper.ClientId = settings.ClientId;
				TokenHelper.ClientSecret = settings.ClientSecret;
				TokenHelper.HostedAppHostName = settings.HostedAppHostName;
				TokenHelper.HostedAppHostNameOverride = settings.HostedAppHostNameOverride;
				TokenHelper.Realm = settings.Realm;
				TokenHelper.SecondaryClientSecret = settings.SecondaryClientSecret;
				TokenHelper.ServiceNamespace = TokenHelper.Realm;
			}

			if (authSettings is IHighTrustSettings)
			{
				var settings = (IHighTrustSettings)authSettings;

				TokenHelper.ClientId = settings.ClientId;
				TokenHelper.ClientSigningCertificatePath = settings.ClientSigningCertificatePath;
				TokenHelper.ClientSigningCertificatePassword = settings.ClientSigningCertificatePassword;
				TokenHelper.IssuerId = string.IsNullOrEmpty(settings.IssuerId) ? TokenHelper.ClientId : settings.IssuerId;
				TokenHelper.ClientCertificate = (string.IsNullOrEmpty(TokenHelper.ClientSigningCertificatePath) || string.IsNullOrEmpty(TokenHelper.ClientSigningCertificatePassword))
					? null
					: new X509Certificate2(TokenHelper.ClientSigningCertificatePath, TokenHelper.ClientSigningCertificatePassword);

				TokenHelper.SigningCredentials = (TokenHelper.ClientCertificate == null) ? null
					: new X509SigningCredentials(TokenHelper.ClientCertificate, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);

			}
		}

		protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
		{
			var state = Request.Query["state"];

			var properties = Options.StateDataFormat.Unprotect(state);
			if (properties == null)
			{
				return AuthenticateResult.Fail("The oauth state was missing or invalid.");
			}

			if (!ValidateCorrelationId(properties))
			{
				return AuthenticateResult.Fail("Correlation failed.");
			}

			ClaimsIdentity identity;

			if (Context.User.Identity.IsAuthenticated &&
				Context.User.Identity.AuthenticationType == Options.AuthenticationScheme)
			{
				identity = (ClaimsIdentity)Context.User.Identity;
			}
			else
			{
				identity = new ClaimsIdentity(Options.AuthenticationScheme);
			}

			Uri spHostUrl;
			if (!Uri.TryCreate(Request.Query[SPAddinAuthenticationDefaults.HostUrlKey], UriKind.Absolute, out spHostUrl))
			{
				throw new Exception("Can not get host url from query string");
			}

			Uri spAppWebUrl;
			if (Uri.TryCreate(Request.Query[SPAddinAuthenticationDefaults.AppWebUrlKey], UriKind.Absolute, out spAppWebUrl))
			{
				identity.AddClaim(new Claim(SPAddinClaimTypes.SPAppWebUrl, spAppWebUrl.AbsoluteUri));
			}

			string accessToken;

			if (TokenHelper.IsHighTrustApp())
			{
				//TODO
				throw new NotImplementedException("S2S authenticaiton is not implemented yet");
			}
			else
			{
				var contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);
				var contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, new Uri(CurrentUri).Authority);

				identity.AddClaim(new Claim(SPAddinClaimTypes.RefreshToken, contextToken.RefreshToken));
				identity.AddClaim(new Claim(SPAddinClaimTypes.Realm, contextToken.Realm));
				identity.AddClaim(new Claim(SPAddinClaimTypes.TargetPrincipalName, contextToken.TargetPrincipalName));
				identity.AddClaim(new Claim(SPAddinClaimTypes.CacheKey, contextToken.CacheKey));

				accessToken = TokenHelper.GetAcsAccessToken(contextToken.RefreshToken, contextToken.TargetPrincipalName, spHostUrl.Authority, contextToken.Realm);
			}

			var ticket = CreateTicket(identity, properties, accessToken, spHostUrl);

			if (ticket != null)
			{
				((ClaimsIdentity)ticket.Principal.Identity).AddClaim(new Claim(SPAddinClaimTypes.SPAddinAuthentication, "1"));
				return AuthenticateResult.Success(ticket);
			}

			return AuthenticateResult.Fail("Failed to retrieve user information from remote server.");
		}

		protected AuthenticationTicket CreateTicket(ClaimsIdentity identity, AuthenticationProperties properties, string accessToken, Uri spHostUrl)
		{
			var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Options.AuthenticationScheme);

			using (var clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl.AbsoluteUri, accessToken))
			{
				var user = clientContext.Web.CurrentUser;
				clientContext.Load(user);
				clientContext.ExecuteQuery();

				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.LoginName, null, Options.SignInScheme));
				identity.AddClaim(new Claim(ClaimTypes.Name, user.Title));
				identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
				identity.AddClaim(new Claim(SPAddinClaimTypes.SPHostUrl, spHostUrl.AbsoluteUri));

				Options.Events.OnAuthenticated(new SPAddinOnAuthenticatedContext(Context, user, identity));
			}

			return ticket;
		}

		protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var properties = new AuthenticationProperties(context.Properties);

			if (string.IsNullOrEmpty(properties.RedirectUri))
			{
				properties.RedirectUri = CurrentUri;
			}

			var hostUrl = new Uri(properties.Items[SPAddinAuthenticationDefaults.HostUrlKey]);
			properties.Items.Remove(SPAddinAuthenticationDefaults.HostUrlKey);

			GenerateCorrelationId(properties);

			var uriBuilder = new UriBuilder(CurrentUri)
			{
				Path = Options.CallbackPath.Value
			};

			var stateString = Options.StateDataFormat.Protect(properties);
			var postRedirectUrl = uriBuilder.Uri.GetLeftPart(UriPartial.Path) + "?{StandardTokens}&SPAppWebUrl={SPAppWebUrl}&state=" + stateString;

			var redirectUri = TokenHelper.GetAppContextTokenRequestUrl(hostUrl.AbsoluteUri, WebUtility.UrlEncode(postRedirectUrl));

			Options.Events.OnRedirectToAuthorizationEndpoint(new SPAddinRedirectToSharePointContext(Context, properties, redirectUri));

			return Task.FromResult(true);
		}
	}
}
