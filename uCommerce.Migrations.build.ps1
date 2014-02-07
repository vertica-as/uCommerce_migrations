$Global:base_dir = resolve-path .

properties {
    $configuration = 'Debug'
	$release_path = "$Global:base_dir\release"
}

task default -depends Clean, Compile

task Clean {
    exec { msbuild .\uCommerce.Migrations.sln /t:clean /p:configuration=$configuration /m }
    Remove-Item $release_path -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
}

task Compile {
    exec { msbuild .\uCommerce.Migrations.sln /p:configuration=$configuration /m }

}