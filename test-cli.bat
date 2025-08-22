@echo off
echo Testing CLI mode of Overlay application
echo.

echo Displaying "Hello World" at position (200, 200) for 5 seconds
dotnet run --project Overlay.Cli -- -x 200 -y 200 -text "Hello World" -duration 5

timeout /t 2 /nobreak >nul

echo Displaying "CLI Test" at position (400, 400) for 3 seconds
dotnet run --project Overlay.Cli -- -x 400 -y 400 -text "CLI Test" -duration 3

timeout /t 2 /nobreak >nul

echo Displaying "Scripted Overlay" at position (400, 600) for 4 seconds
dotnet run --project Overlay.Cli -- -x 400 -y 600 -text "Scripted Overlay" -duration 4

echo.
echo Test complete!