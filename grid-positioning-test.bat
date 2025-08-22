@echo off
echo Testing grid positioning features
echo.

echo 1. Displaying text in column 0, row 0 (top left) on primary display
dotnet run --project Overlay.Cli -- -text "TOP LEFT" -duration 5 -foreground "#FF0000" -dropshadow "#000000" -display 1 -column 0 -row 0

timeout /t 2 /nobreak >nul

echo 2. Displaying text in column 2, row 2 (bottom right) on primary display
dotnet run --project Overlay.Cli -- -text "BOTTOM RIGHT" -duration 5 -foreground "#00FF00" -dropshadow "#000000" -display 1 -column 2 -row 2

timeout /t 2 /nobreak >nul

echo 3. Displaying text in column 1, row 1 (center) on primary display
dotnet run --project Overlay.Cli -- -text "CENTER" -duration 5 -foreground "#0000FF" -dropshadow "#FFFF00" -display 1 -column 1 -row 1

timeout /t 2 /nobreak >nul

echo 4. Displaying text in column 0, row 0 on vertical display (display 2)
dotnet run --project Overlay.Cli -- -text "VERTICAL TOP" -duration 5 -foreground "#FF00FF" -dropshadow "#00FFFF" -display 2 -column 0 -row 0

timeout /t 2 /nobreak >nul

echo 5. Displaying text in column 0, row 2 on vertical display (display 2)
dotnet run --project Overlay.Cli -- -text "VERTICAL BOTTOM" -duration 5 -foreground "#00FFFF" -dropshadow "#FF00FF" -display 2 -column 0 -row 2

echo.
echo All grid positioning tests completed!