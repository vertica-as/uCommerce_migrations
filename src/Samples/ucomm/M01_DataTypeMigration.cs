/*using FluentMigrator;
using UCommerce.EntitiesV2;
using uCommerce.Migrations.Runner_FluentMigrator;

namespace uCommerce.Migrations.Samples.ucomm
{
	[Migration(1)]
	public class M01_DataTypeMigration : uCommerceMigration
	{
		protected override void MigrateUp()
		{
			//System.Diagnostics.Debugger.Launch();

			Definition justImportedCategory = Migrator.CategoryDefinition("MigratedDefinition", "This category was created using uCommerce Migrations.");
			Migrator.Save(justImportedCategory);
		}

		protected override void MigrateDown()
		{
			Migrator.DeleteCategoryDefinition("MigratedDefinition");
		}
	}
}*/