#NuGet本体のアップデート
cd ..
.\build\nuget update -self

$Packages = ls -include packages.config -recurse

#パッケージのアップデート(要repositories.config)
cd .\packages
$Packages | foreach { ..\build\nuget update -verbose $_.FullName}
cd ..\build

#パッケージのインストール
$Packages | foreach { .\nuget install $_.FullName -o ..\packages}