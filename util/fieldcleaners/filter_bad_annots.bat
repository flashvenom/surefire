@echo off
:: Drag-and-drop handler for filter_bad_annots.py
if "%~1"=="" (
    echo Drag a PDF onto this file to process it.
    pause
    goto :eof
)

:: Call the Python script that sits in the same folder as this BAT
python "%~dp0filter_bad_annots.py" "%~1"
pause
