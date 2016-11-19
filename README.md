# SharePoint add-in Asp.Net authentication
Authenticate SharePoint add-ins in Asp.Net applications using modern middleware approach.   

Asp.Net versions supported:  
 - Asp.Net MVC 5
 - Asp.Net Core (coming soon)

Add-in types supported: 
 - Low-trust (SharePoint Online, SharePoint 2013\2016 in low trust scenario, OAuth authentication)
 - High-trust (SharePoint 2013\2016, S2S authentication) with integrated windows authentication (ADFS currently is not supported)

## Install package via Nuget

Asp.Net MVC 5:
```
Install-Package AspNet.Owin.SharePoint.Addin
```

Or Asp.Net Core (coming soon):
```
Install-Package AspNet.Core.SharePoint.Addin
```

Explicitly install one of the SharePoint client libraries:
```bash
Install-Package Microsoft.SharePointOnline.CSOM 
#OR
Install-Package Microsoft.SharePoint2013.CSOM
#OR
Install-Package Microsoft.SharePoint2016.CSOM 
```


## Sample using

#### Asp.Net MVC 5:  

`Startup.cs`
```csharp
public void Configuration(IAppBuilder app)
{
	var cookieAuth = new CookieAuthenticationOptions
	{
		LoginPath = new PathString("/Auth/Login"),
		Provider = new AdddInCookieAuthenticationProvider()
	};

	app.SetDefaultSignInAsAuthenticationType(cookieAuth.AuthenticationType);
	app.UseCookieAuthentication(cookieAuth);

	app.UseSPAddinAuthentication(new SPAddInAuthenticationOptions
	{
		ClientId = ConfigurationManager.AppSettings["ClientId"]
	});
}

```
`HomeController.cs`
```csharp
[Authorize]
public class HomeController : Controller
{
	// GET: Home
	public ActionResult Index()
	{
		var spContext = SPContextProvider.Get(User as ClaimsPrincipal);
		using (var ctx = spContext.CreateUserClientContextForSPHost())
		{
			ctx.Load(ctx.Web.CurrentUser);
			ctx.ExecuteQuery();
		}
		return View();
	}
}
```
#### Asp.Net Core (coming soon)

To make it work, you also need `AuthController` configured to perform authentication chanllenge when the user is not yet authenticated or authentication is expired. 
This repository contains samples for using SharePoint middleware, **I highly recommend you to configure samples** on your environment and run them in order to have better udnerstanding around how all the pieces fit together. 

Use [wiki](https://github.com/s-KaiNet/SharePoint-AspNet-Authentication/wiki) to setup samples. 