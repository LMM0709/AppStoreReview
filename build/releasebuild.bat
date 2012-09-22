powershell .\NugetRestore.ps1
@IF %ERRORLEVEL% NEQ 0 GOTO err
cd ..
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release
cd .\build
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1