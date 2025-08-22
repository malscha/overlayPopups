@echo off
echo Building Overlay CLI executables
echo.

echo Building lightweight version (requires .NET 8 on target machine)...
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish/Overlay.Cli ./Overlay.Cli/Overlay.Cli.csproj
if %errorlevel% neq 0 (
    echo Error building lightweight version
    exit /b %errorlevel%
)

echo.
echo Building self-contained version (completely portable)...
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/Overlay.Cli.SelfContained ./Overlay.Cli/Overlay.Cli.csproj
if %errorlevel% neq 0 (
    echo Error building self-contained version
    exit /b %errorlevel%
)

echo.
echo Build complete!
echo.
echo Lightweight version: publish\Overlay.Cli\Overlay.Cli.exe
echo Self-contained version: publish\Overlay.Cli.SelfContained\Overlay.Cli.exe