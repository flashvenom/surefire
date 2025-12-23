@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "PS_SCRIPT=%SCRIPT_DIR%build-installer.ps1"

powershell -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
set "EXITCODE=%ERRORLEVEL%"

echo.
if not "%CI%"=="" exit /b %EXITCODE%
pause
exit /b %EXITCODE%

