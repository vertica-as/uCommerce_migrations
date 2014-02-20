$Global:base_dir = resolve-path .

properties {
	$configuration = 'Debug'
	$release_path = "$Global:base_dir\release"
}

task default -depends Clean, Compile, Deploy, Pack

task Clean {
	exec { msbuild .\uCommerce.Migrations.sln /t:clean /p:configuration=$configuration /m }
	Remove-Item $release_path -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
}

task Compile {
	exec { msbuild .\uCommerce.Migrations.sln /p:configuration=$configuration /m }
}

task Deploy {
	$release_folder = Ensure-Release-Folder $release_path

	$core = Src-Folder $Global:base_dir "Core"
	$fluentMigrator = Src-Folder $Global:base_dir "Runner_FluentMigrator"

	Get-ChildItem -Path "$core" -Filter "*.cs" |
		Copy-Item -Destination $release_folder

	Get-ChildItem -Path "$core" -Filter "*.config.pp" |
		Copy-Item -Destination $release_folder

	Get-ChildItem -Path "$fluentMigrator" -Filter "*.cs" |
		Copy-Item -Destination $release_folder

	Get-ChildItem $Global:base_dir -Filter '*.nuspec' |
		Copy-Item -Destination $release_path

	Copy-Item "$Global:base_dir\README.md" -Destination "$release_path\README.txt"
}

task Pack {
	Ensure-Release-Folder $release_path

	$nuget = "$Global:base_dir\tools\nuget\nuget.exe"

	Get-ChildItem -File -Filter '*.nuspec' -Path $release_path  | 
		ForEach-Object { exec { & $nuget pack $_.FullName /o $release_path } }
}

function Ensure-Release-Folder($base)
{
	$release_folder = "$base\content\Infrastructure"

	md $release_folder -Force | Out-Null

	return $release_folder
}

function Src-Folder($base, $name)
{
	return "$base\src\$name\"
}