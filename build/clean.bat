cd ..
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:clean /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 GOTO err
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:clean /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 GOTO err
cd .\build
powershell .\ProjectClean.ps1
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1