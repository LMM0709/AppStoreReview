#NuGet�{�̂̃A�b�v�f�[�g
cd ..
.\build\nuget update -self

$Packages = ls -include packages.config -recurse

#�p�b�P�[�W�̃A�b�v�f�[�g(�vrepositories.config)
cd .\packages
$Packages | foreach { ..\build\nuget update -verbose $_.FullName}
cd ..\build

#�p�b�P�[�W�̃C���X�g�[��
$Packages | foreach { .\nuget install $_.FullName -o ..\packages}