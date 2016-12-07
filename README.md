# SharePoint add-in Asp.Net authentication [![analytics](http://www.google-analytics.com/collect?v=1&t=pageview&tid=UA-87971440-5&cid=53bf5757-8d4f-44b3-94c1-1a0f33942f48&dl=https%3A%2F%2Fgithub.com%2Fs-KaiNet%2FSharePoint-AspNet-Authentication)]()  
Authenticate SharePoint add-ins in Asp.Net applications using modern middleware approach.   

Asp.Net versions supported:  
 - Asp.Net MVC 5
 - Asp.Net Core (MVC 6)

Add-in types supported: 
 - Low-trust (SharePoint Online, SharePoint 2013\2016 in low trust scenario, OAuth authentication)
 - High-trust (SharePoint 2013\2016, S2S authentication) with integrated windows authentication on IIS or ADFS authentication (high-trust for Asp.Net Core is not yet implemented)

## Install package via Nuget

Asp.Net MVC 5:
```
Install-Package AspNet.Owin.SharePoint.Addin.Authentication
```

Or Asp.Net Core:
```
Install-Package AspNet.Core.SharePoint.Addin.Authentication
```

**NOTES on Asp.Net Core:** `AspNet.Core.SharePoint.Addin.Authentication` uses full 4.5.1 .NET Framework and currently I don't have plans to port to the .NET Core, because it requires **a lot** of modifications and rewriting `TokenHelper` almost from scratch.


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
#### Asp.Net Core 
`Startup.cs`
```csharp
public void ConfigureServices(IServiceCollection services)
{
	.....
	services.AddOptions();
	services.Configure<LowTrustSettings>(Configuration.GetSection("SharePoint"));
	.....
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
{
	....
	app.UseCookieAuthentication(new CookieAuthenticationOptions
	{
		LoginPath = "/Auth/Login",
		AutomaticAuthenticate = true,
		AutomaticChallenge = true,
		Events = new CustomCookieEvents(serviceProvider)
	});

	app.UseSPAddinAuthentication(new SPAddinAuthenticationOptions
	{
		AutomaticAuthenticate = false,
		AutomaticChallenge = false,
		SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
		AuthenticationScheme = SPAddinAuthenticationDefaults.AuthenticationType,
		AuthSettings = serviceProvider.GetService<IOptions<LowTrustSettings>>().Value
	});
	....
}
```

`HomeController.cs`
```csharp
[Authorize]
public class HomeController : Controller
{
	public IActionResult Index()
	{
		var spcontext = SPContextProvider.Get(User);

		using (var clientContext = spcontext.CreateUserClientContextForSPHost())
		{
			clientContext.Load(clientContext.Web.CurrentUser);
			clientContext.Load(clientContext.Web);
			clientContext.ExecuteQuery();

			ViewBag.User = clientContext.Web.CurrentUser.LoginName;
			ViewBag.Host = clientContext.Web.Title;
		}
		return View();
	}
}
```

To make it work, you also need `AuthController` configured to perform authentication challenge when the user is not yet authenticated or authentication is expired. 
This repository contains samples for using SharePoint middleware, **I highly recommend you to configure samples** on your environment and run them in order to have better understanding around how all the pieces fit together. 

Use [wiki](https://github.com/s-KaiNet/SharePoint-AspNet-Authentication/wiki) to setup samples. 