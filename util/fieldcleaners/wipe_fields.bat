@echo off
if "%~1"=="" (
  echo Drag a PDF onto this file to strip all form fields.
  pause
  goto :eof
)
python "%~dp0wipe_fields.py" "%~1"
pause
