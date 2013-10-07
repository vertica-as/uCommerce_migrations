﻿[CmdletBinding()]
Param(
	[Parameter(Mandatory=$False)]
	[string]$deploy_folder = ".\Deploy\",
	[Parameter(Mandatory=$False)]
	[string]$package_folder = ".\packages\FluentMigrator.1.1.1.0\tools\",
    [Parameter(Mandatory=$False)]
	[string]$ucommerce_folder = ".\lib\UCommerce-4.0.2.13277\",
    [Parameter(Mandatory=$False)]
	[string]$migrations_folder = ".\src\Samples\bin\debug\",
    [Parameter(Mandatory=$False)]
	[string]$migration_database = "ucomm",
    [Parameter(Mandatory=$False)]
	[string]$migration_server = ".\SQLEXPRESS",
	[Parameter(Mandatory=$False)]
	[ValidateSet('migrate', 'migrate:down', 'rollback', 'rollback:toversion', 'rollback:all')]
	[string]$task = "migrate",
	[Parameter(Mandatory=$False)]
	[int64]$version = 0
)

function Run-Me
{
    EnsureDeploy
    CopyMigrationsFramework
    CopyUCommerce    
    CopyMigrations
    Migrate
}

function EnsureDeploy() {
    New-Item -ItemType Directory $deploy_folder -Force
}

function CopyUCommerce(){
    $all_files = $ucommerce_folder + "*"
    Copy-Item $all_files $deploy_folder -Force
}

function CopyMigrationsFramework(){
    @(
        ".\src\Core\bin\Debug\uCommerce.Migrations.Core.dll",
        ".\src\Core\bin\Debug\Components.config",
        ".\src\Runner_FluentMigrator\bin\Debug\uCommerce.Migrations.Runner_FluentMigrator.dll"
        $package_folder + "*"
    ) | Copy-Item -Destination $deploy_folder -Force
}

function CopyMigrations(){
    $migrations_dll = $migrations_folder + "uCommerce.Migrations.Samples.dll"
    Copy-Item $migrations_dll $deploy_folder
}

function Migrate() {
    $migrations_dll = $deploy_folder + "uCommerce.Migrations.Samples.dll"
    $migrate = $deploy_folder + "Migrate.exe"
    $connectionString = "Integrated Security=SSPI;Initial Catalog=$migration_database;Data Source=$migration_server"

    & $migrate -db SqlServer2012 -a $migrations_dll -conn $connectionString -t $task
}



Run-Me