@IF %ERRORLEVEL% NEQ 0 GOTO err
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1