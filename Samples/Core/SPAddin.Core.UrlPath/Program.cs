using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace SPAddin.Core.UrlPath
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel(options => {
					options.UseHttps(@"c:\Certs\sp2013dev_ssl.pfx", "QazWsx123");
					options.NoDelay = true;
				})
				.UseUrls("https://localhost:44390")
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}
