using FluentMigrator;
using uCommerce.Migrations.Core;

namespace uCommerce.Migrations.Runner_FluentMigrator
{
	public abstract class uCommerceMigration : Migration
	{
		protected Migrator Migrator { get; private set; }

		public override void Up()
		{
			Migrator = new Migrator(ConnectionString);

			MigrateUp();
		}

		protected abstract void MigrateUp();

		public override void Down()
		{
			Migrator = new Migrator(ConnectionString);

			MigrateDown();
		}

		protected abstract void MigrateDown();
	}
}