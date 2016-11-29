using System;
using System.Linq;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using SPAddinOwin.Data.DB;
using SPAddinOwin.Data.Entities;

namespace SPAddinOwin.Sample.ADFS.Common
{
	public class NavigationManager
	{
		private readonly IOwinContext _owinContext;

		public NavigationManager(IOwinContext context)
		{
			_owinContext = context;
		}

		public Host EnsureHostByUrl(string hosturl)
		{
			hosturl = EnsureTrailingSlash(hosturl);
			var hostUrlHash = GetSha256Hash(hosturl);
			var addinDbContext = _owinContext.Get<AddInContext>();

			var host = addinDbContext.Hosts.Where(h => h.Hash == hostUrlHash).ToList().SingleOrDefault();
			if (host == null)
			{
				var allHosts = addinDbContext.Hosts.ToList().Select(h => h.ShortHandUrl).ToList();
				var shortHandUrl = GetRandomString();
				while (allHosts.Contains(shortHandUrl))
				{
					shortHandUrl = GetRandomString();
				}

				host = new Host
				{
					Hash = hostUrlHash,
					ShortHandUrl = shortHandUrl,
					Url = hosturl
				};

				addinDbContext.Hosts.Add(host);
				addinDbContext.SaveChanges();
			}

			return host;
		}

		public Host GetHostByShortHandUrl(string shortUrl)
		{
			var addinDbContext = _owinContext.Get<AddInContext>();
			return addinDbContext.Hosts.Where(h => h.ShortHandUrl == shortUrl).ToList().SingleOrDefault();
		}

		private string GetRandomString()
		{
			var chars = "abcdefghijklmnopqrstuvwxyz";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private string GetSha256Hash(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			using (var sha = new System.Security.Cryptography.SHA256Managed())
			{
				byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
				byte[] hash = sha.ComputeHash(textData);
				return BitConverter.ToString(hash).Replace("-", string.Empty);
			}
		}

		private string EnsureTrailingSlash(string url)
		{
			if (!string.IsNullOrEmpty(url) && url[url.Length - 1] != '/')
			{
				return url + "/";
			}

			return url;
		}
	}
}