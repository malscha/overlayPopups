@echo off
REM Overlay CLI Launcher
REM This script runs the published Overlay.Cli executable with the provided arguments

REM Check if the executable exists
if not exist "publish\Overlay.Cli\Overlay.Cli.exe" (
    echo Error: Overlay.Cli.exe not found. Please run 'dotnet publish' first.
    exit /b 1
)

REM Run the Overlay CLI with all provided arguments
publish\Overlay.Cli\Overlay.Cli.exe %*