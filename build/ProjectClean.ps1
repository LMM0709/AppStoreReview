cd ..

#�T�u�f�B���N�g�����܂߂�packages�t�H���_������
$PackagesDir = ls -recurse | ?{($_.Name -eq "packages")}

#packages�t�H���_���̃t�H���_��S�폜
$PackagesDir | ls | ?{($_.Mode -eq "d----")} | rm -recurse -force

#�T�u�f�B���N�g�����܂߂�bin��obj�t�H���_��S�폜
ls -recurse | ?{($_.Name -eq "bin") -or ($_.Name -eq "obj")} | rm -recurse -force

#�g���q*.bak�̃t�@�C�����폜
ls -include *.bak -recurse | rm -recurse -force

cd .\build