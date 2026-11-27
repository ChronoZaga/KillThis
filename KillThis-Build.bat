@echo off
setlocal
cd /d "%~dp0"

rem Version number for the built EXE (Major.Minor.Build.Revision)
set "VERSION=2.0.0.0"

set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo Compiler not found: %CSC%
    pause
    exit /b 1
)

set "ICON="
if exist "killthis.ico" set "ICON=/win32icon:killthis.ico"

set "VERFILE=%TEMP%\KillThis_Version.cs"
> "%VERFILE%" echo [assembly: System.Reflection.AssemblyVersion("%VERSION%")]
>> "%VERFILE%" echo [assembly: System.Reflection.AssemblyFileVersion("%VERSION%")]

"%CSC%" /nologo /platform:x64 /target:winexe /optimize+ %ICON% /out:KillThis.exe KillThis.cs "%VERFILE%"
set "RESULT=%ERRORLEVEL%"
del "%VERFILE%" 2>nul

if not "%RESULT%"=="0" (
    echo Build failed.
) else (
    echo Built KillThis.exe v%VERSION% ^(x64^)
    if not exist "KillThis.txt" (
        type nul > "KillThis.txt"
        echo Created empty KillThis.txt
    ) else (
        echo KillThis.txt already exists - left unchanged
    )
)
pause
