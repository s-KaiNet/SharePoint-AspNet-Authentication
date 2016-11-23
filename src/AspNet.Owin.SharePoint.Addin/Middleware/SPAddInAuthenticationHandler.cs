using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Owin.SharePoint.Addin.Authentication.Common;
using AspNet.Owin.SharePoint.Addin.Authentication.Provider;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace AspNet.Owin.SharePoint.Addin.Authentication.Middleware
{
	public class SPAddInAuthenticationHandler : AuthenticationHandler<SPAddInAuthenticationOptions>
	{
		protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
		{
			var identity = new ClaimsIdentity(Options.SignInAsAuthenticationType);

			Uri spHostUrl;
			if (!Uri.TryCreate(Request.Query[SharePointContext.SPHostUrlKey], UriKind.Absolute, out spHostUrl))
			{
				throw new Exception("Can not get host url from query string");
			}

			Uri spAppWebUrl;
			if (Uri.TryCreate(Request.Query[SharePointContext.SPAppWebUrlKey], UriKind.Absolute, out spAppWebUrl))
			{
				identity.AddClaim(new Claim(CustomClaimTypes.SPAppWebUrl, spAppWebUrl.AbsoluteUri));
			}

			string accessToken;
			
			if (AuthHelper.IsHighTrustApp())
			{
				var userId = AuthHelper.GetWindowsUserId(Context);
				accessToken = AuthHelper.GetS2SAccessToken(spHostUrl, userId);

				identity.AddClaim(new Claim(CustomClaimTypes.ADUserId, userId));
				identity.AddClaim(new Claim(CustomClaimTypes.CacheKey, userId));
				identity.AddClaim(new Claim(CustomClaimTypes.Realm, AuthHelper.GetRealmFromTargetUrl(spHostUrl)));
			}
			else
			{
				var contextTokenString = AuthHelper.GetContextTokenFromRequest(Request);
				var contextToken = AuthHelper.ReadAndValidateContextToken(contextTokenString, Request.Uri.Authority);

				identity.AddClaim(new Claim(CustomClaimTypes.RefreshToken, contextToken.RefreshToken));
				identity.AddClaim(new Claim(CustomClaimTypes.Realm, contextToken.Realm));
				identity.AddClaim(new Claim(CustomClaimTypes.TargetPrincipalName, contextToken.TargetPrincipalName));
				identity.AddClaim(new Claim(CustomClaimTypes.CacheKey, contextToken.CacheKey));

				accessToken = AuthHelper.GetAcsAccessToken(contextToken.RefreshToken, contextToken.TargetPrincipalName, spHostUrl.Authority, contextToken.Realm);
			}

			return await CreateTicket(accessToken, identity, spHostUrl);
		}

		protected override Task ApplyResponseChallengeAsync()
		{
			if (Response.StatusCode == 401)
			{
				var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

				if (challenge == null)
				{
					return Task.FromResult<object>(null);
				}

				var state = challenge.Properties;

				var hostUrl = new Uri(state.Dictionary[SharePointContext.SPHostUrlKey]);

				var uriBuilder = new UriBuilder(Request.Uri)
				{
					Path = Options.CallbackPath.Value
				};

				state.Dictionary.Remove(SharePointContext.SPHostUrlKey);
				var stateString = Options.StateDataFormat.Protect(state);
				var postRedirectUrl = uriBuilder.Uri.GetLeftPart(UriPartial.Path) + "?{StandardTokens}&SPAppWebUrl={SPAppWebUrl}&state=" + stateString;

				var redirectUri = AuthHelper.GetAppContextTokenRequestUrl(hostUrl.AbsoluteUri, WebUtility.UrlEncode(postRedirectUrl));

				Response.Redirect(redirectUri);
			}

			return Task.FromResult<object>(null);
		}

		public override async Task<bool> InvokeAsync()
		{
			if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
			{
				if (AuthHelper.IsHighTrustApp())
				{
					var logonUserIdentity = AuthHelper.GetWindowsUser(Context);
					// If not authenticated or already authenticated using cookies, current identity will be the IIS App Pool, must re-authenticate.
					if (!logonUserIdentity.IsAuthenticated)
					{
						Response.StatusCode = 418;
						// Prevent further processing by the owin pipeline.
						return true;
					}
				}
				var ticket = await AuthenticateAsync();

				if (ticket != null)
				{
					Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

					Response.Redirect(ticket.Properties.RedirectUri);

					// Prevent further processing by the owin pipeline.
					return true;
				}
			}
			// Let the rest of the pipeline run.
			return false;
		}

		private async Task<AuthenticationTicket> CreateTicket(string accessToken, ClaimsIdentity identity, Uri spHostUrl)
		{
			using (var clientContext = AuthHelper.GetClientContextWithAccessToken(spHostUrl.AbsoluteUri, accessToken))
			{
				var user = clientContext.Web.CurrentUser;
				clientContext.Load(user);
				clientContext.ExecuteQuery();

				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.LoginName, null, Options.AuthenticationType));
				identity.AddClaim(new Claim(ClaimTypes.Name, user.Title));
				identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
				identity.AddClaim(new Claim(CustomClaimTypes.SPHostUrl, spHostUrl.AbsoluteUri));

				var properties = Options.StateDataFormat.Unprotect(Request.Query["state"]);

				await Options.Provider.Authenticated(new SPAddinAuthenticatedContext(Context, user, identity));

				return new AuthenticationTicket(identity, properties);
			}
		}
	}
}
