$Global:base_dir = resolve-path .

properties {
	$configuration = 'Debug'
	$release_path = "$Global:base_dir\release"
}

task default -depends Clean, Compile, Deploy

task Clean {
	exec { msbuild .\uCommerce.Migrations.sln /t:clean /p:configuration=$configuration /m }
	Remove-Item $release_path -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
}

task Compile {
	exec { msbuild .\uCommerce.Migrations.sln /p:configuration=$configuration /m }
}

task Deploy {
	$release_folders = Ensure-Release-Folders  $release_path
}

function Ensure-Release-Folders($base)
{
	$release_folders = ($base, "$base\content\")

	foreach ($f in $release_folders) { md $f -Force | Out-Null }

	return $release_folders
}