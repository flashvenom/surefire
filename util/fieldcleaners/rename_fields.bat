@echo off
if "%~1"=="" (
  echo Drag a PDF onto this file to rename all its form fields.
  pause
  goto :eof
)
python "%~dp0rename_fields.py" "%~1"
pause
