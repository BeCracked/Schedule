@echo OFF

setlocal ENABLEEXTENSIONS
:: Name of the project file. e.g. Project.shfbproj
set PROJECT_FILE=%1
:: Configuration to compile with
set CONFIGURATION=%2
:: Command line arguments for Sandcastle
set CMA = /p:CONFIGURATION=%CONFIGURATION%
:: MSBuild Version and Path
set MSBUILD_VERSION=4.0
set KEY_NAME="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\%MSBUILD_VERSION%"
set VALUE_NAME=MSBuildToolsPath

FOR /F "usebackq skip=1 tokens=1-2*" %%A IN (`REG QUERY %KEY_NAME% /v %VALUE_NAME% 2^>nul`) DO (
    set ValueName=%%A
    set ValueType=%%B
    set ValueValue=%%C
)
    echo Compiling %PROJECT_FILE% with MSBuild-Version %MSBUILD_VERSION%
    :: Run MSBuild
    %ValueValue%MSBuild.exe %CMA% %PROJECT_FILE%