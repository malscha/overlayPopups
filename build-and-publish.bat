@echo off
echo Building and Publishing Overlay Application
echo ==========================================

echo Restoring packages...
dotnet restore

echo Building solution...
dotnet build --configuration Release

echo Publishing CLI application...
dotnet publish Overlay.Cli -c Release -r win-x64 --self-contained -o ./publish/cli

echo Publishing GUI application...
dotnet publish Overlay.App -c Release -r win-x64 --self-contained -o ./publish/gui

echo.
echo Build and publish complete!
echo.
echo CLI application published to: publish\cli
echo GUI application published to: publish\gui
echo.
echo To run CLI application:
echo   cd publish\cli
echo   Overlay.Cli.exe [options]
echo.
echo Documentation files created:
echo   - README.md      (User guide)
echo   - IMPLEMENTATION.md (Developer guide)
echo   - CHANGELOG.md    (Version history)
echo   - LICENSE         (MIT License)