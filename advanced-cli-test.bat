@echo off
echo Testing advanced CLI features
echo.

echo 1. Displaying strobing purple text with pink drop shadow on display 2 (centered) for 10 seconds
dotnet run --project Overlay.Cli -- -text "STROBING PURPLE" -duration 10 -foreground "#800080" -dropshadow "#FF69B4" -strobing -display 1 -center

timeout /t 2 /nobreak >nul

echo 2. Displaying red text with yellow drop shadow on primary display for 5 seconds
dotnet run --project Overlay.Cli -- -text "RED TEXT" -duration 5 -foreground "#FF0000" -dropshadow "#FFFF00" -x 300 -y 300

timeout /t 2 /nobreak >nul

echo 3. Displaying blue text with green drop shadow on display 1 (centered) for 7 seconds
dotnet run --project Overlay.Cli -- -text "BLUE TEXT" -duration 7 -foreground "#0000FF" -dropshadow "#00FF00" -display 1 -center

timeout /t 2 /nobreak >nul

echo 4. Displaying orange strobing text on primary display for 6 seconds
dotnet run --project Overlay.Cli -- -text "ORANGE STROBE" -duration 6 -foreground "#FFA500" -dropshadow "#000000" -strobing -x 400 -y 500

timeout /t 2 /nobreak >nul

echo 5. Displaying text in column 0, row 0 (top left) on primary display
dotnet run --project Overlay.Cli -- -text "GRID POSITION" -duration 5 -foreground "#FF00FF" -dropshadow "#000000" -display 1 -column 0 -row 0

timeout /t 2 /nobreak >nul

echo 6. Displaying rotating texts every 2 seconds for 10 seconds
dotnet run --project Overlay.Cli -- -texts "ROTATE 1;ROTATE 2;ROTATE 3" -duration 10 -switch 2 -foreground "#00FFFF" -dropshadow "#000000" -display 1 -column 1 -row 1

echo.
echo All tests completed!