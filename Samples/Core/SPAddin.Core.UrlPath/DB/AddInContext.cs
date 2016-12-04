using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SPAddin.Core.UrlPath.Entities;

namespace SPAddin.Core.UrlPath.DB
{
	public class AddInContext : DbContext
	{
		public DbSet<Host> Hosts { get; set; }

		public AddInContext(DbContextOptions<AddInContext> options)
			: base(options)
		{
			
		}
	}
}
