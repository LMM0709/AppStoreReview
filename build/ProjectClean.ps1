cd ..

#サブディレクトリも含めてpackagesフォルダを検索
$PackagesDir = ls -recurse | ?{($_.Name -eq "packages")}

#packagesフォルダ内のフォルダを全削除
$PackagesDir | ls | ?{($_.Mode -eq "d----")} | rm -recurse -force

#サブディレクトリも含めてbinとobjフォルダを全削除
ls -recurse | ?{($_.Name -eq "bin") -or ($_.Name -eq "obj")} | rm -recurse -force

#拡張子*.bakのファイルを削除
ls -include *.bak -recurse | rm -recurse -force

cd .\build