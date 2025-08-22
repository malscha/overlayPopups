@echo off
echo Testing new text effects in Overlay application
echo.

echo Displaying text with default settings (pink, glowing, medium size, centered)
dotnet run --project Overlay.Cli -- -text "DEFAULT TEST" -duration 5

timeout /t 2 /nobreak >nul

echo Displaying text with outline effect
dotnet run --project Overlay.Cli -- -text "OUTLINE TEST" -duration 5 -column 1 -row 0 -outline

timeout /t 2 /nobreak >nul

echo Displaying text with glow effect
dotnet run --project Overlay.Cli -- -text "GLOW TEST" -duration 5 -column 1 -row 1 -glow

timeout /t 2 /nobreak >nul

echo Displaying text with zoom animation
dotnet run --project Overlay.Cli -- -text "ZOOM TEST" -duration 5 -column 1 -row 2 -zoom

timeout /t 2 /nobreak >nul

echo Displaying text with pulsating effect
dotnet run --project Overlay.Cli -- -text "PULSE TEST" -duration 5 -center -pulse fast

timeout /t 2 /nobreak >nul

echo Displaying small text
dotnet run --project Overlay.Cli -- -text "SMALL TEXT" -duration 5 -center -size small

timeout /t 2 /nobreak >nul

echo Displaying large text
dotnet run --project Overlay.Cli -- -text "LARGE TEXT" -duration 5 -center -size large

timeout /t 2 /nobreak >nul

echo Displaying text with all effects combined
dotnet run --project Overlay.Cli -- -text "ALL EFFECTS" -duration 5 -center -outline -pulse medium -size large

echo.
echo Test complete!