# uCommerce Migrations


Visit the [project website](https://github.com/vertica-as/uCommerce_migrations/) for more information.

Head to [uCommerce website](http://www.ucommerce.net/) to know more about _uCommerce_.

## What is it?

"_uCommerce Migrations_" allows the generation and deletion of _uCommerce_ artifacts from code. It serves as a foundation for which migrations can be created and run using a migration runner. 

### Getting help

For questions or feedback on uCommerce Migrations, please use [uCommerce Migration discussion group](https://groups.google.com/forum/#!forum/ucommerce-migrations).

## uCommerce Migrations for FluentMigrator

"uCommerce Migrations for FluentMigrator" uses [FluentMigrator](https://github.com/schambers/fluentmigrator/wiki) infrastructure to run migrations that maintain _uCommerce_ artifacts.

As of now, it is the preferred way to use _uCommerce migrations_

## Pre-Requisites
* **_uCommerce_ v4.0.2.13277** or above already installed (might work with previous versions, but it has not been verified)
	* **NOTE**: the connection string to the database is needed to run the migrations
	* **NOTE**: no version of _uCommerce_ is distributed with the library, it will have to be downloaded, installed and ready to be reference from your source code.
* A **.Net 4.0 framework** project. The library does not use any four-specific features, so it should work in previous and higher versions of the framework.

## Show me how

Since the purpose of the library is help migrating  _uCommerce_ artifacts, we assume that there is already an installed supported version of _uCommerce_ and that you know the connection string to its database.

### Installation

1. Create a .NET class library project in Visual Studio
2. Add references to the following _uCommerce_-specific assemblies:
	*  UCommerce.dll
	*  UCommerce.Infrastructure.dll
	*  NHibernate.dll
3. Install the nuget package `uCommerce_migrations_runners_FluentMigrator`

### Authoring migrations
Create _FluentMigrator_ migrations that inherit from  `uCommerceMigration`

 For example:

	[Migration(1)]
	public class M01_DataTypeMigration : uCommerceMigration
	{
		protected override void MigrateUp()
		{
			Definition justImportedCategory = Migrator.CategoryDefinition("JustImported", "This category keep products which are just imported by integration.");
			Migrator.Save(justImportedCategory);
		}

		protected override void MigrateDown()
		{
			Migrator.DeleteCategoryDefinition("JustImported");
		}
	}

### Running migrations
_uCommerce migrations_ are run using _FluentMigrator_ runner `Migrate.exe` and providing the database connection to _uCommerce_ database but there are a number of requirements to be fulfilled.

1. Several _uCommerce_-specific assemblies need to be accesible for `Migrate.exe` (usually copied to the same directory)
	* Castle.Core.dll
	* Castle.Windsor.dll (in 5.x versions)
	* NHibernate.dll
	* FluentNHibernate.dll
	* Iesi.Collections.dll
	* Infralution.Licensing.dll	
	* UCommerce.dll
	* UCommerce.Infrastructure.dll
	* Raven.Client.Lightweight (in 7.* versions)
2. The following file needs to be accesible for `Migrate.exe` (usually copied to the same directory)
	* `\Infrastructure\Components.config` 

#### That is quite a manual process isn't it?
Yes.

Since we cannot provide _uCommerce_ within the package, nor we wanted to take a _hard_ dependency on it, these steps are, unfortunately, necessary.

But they do not have to be **manual**.

The way the process can be automatized varies from project, but it always boils down to have some sort of script that:

1. copies the aforementioned artifacts needed to run the migrations
2. Invoke `Migrate.exe`

An example of such migration runner script can be found in `Run-Samples.ps1`

## Not using FluentMigrator

If, for some reason, using FluentMigrator is out of the question, the core nuget package `uCommerce_migrations` can be installed separately.

This package provides the core functionality to migrate artifacts and can be invoked from anywhere.

For example, you might want to run the code to create the necessary artifacts when the _uCommerce_ application starts. But using the migration model is recommended.
