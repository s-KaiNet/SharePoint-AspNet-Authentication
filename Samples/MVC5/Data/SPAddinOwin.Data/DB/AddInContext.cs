using System.Data.Entity;
using SPAddinOwin.Data.Entities;
using SPAddinOwin.Data.Migrations;

namespace SPAddinOwin.Data.DB
{
	public class AddInContext : DbContext
	{
		public DbSet<Host> Hosts { get; set; }

		public static AddInContext Create()
		{
			return new AddInContext();
		}

		public AddInContext() : base("AddInContext")
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<AddInContext, Configuration>("AddInContext"));
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Host>()
				.ToTable("Hosts")
				.HasKey(h => h.Id);
		}
	}
}
