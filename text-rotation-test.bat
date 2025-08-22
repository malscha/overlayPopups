@echo off
echo Testing multiple text rotation feature
echo.

echo 1. Displaying rotating texts every 2 seconds for 10 seconds
dotnet run --project Overlay.Cli -- -texts "TEXT 1;TEXT 2;TEXT 3" -duration 10 -switch 2 -foreground "#FF0000" -dropshadow "#000000" -display 1 -column 1 -row 1

timeout /t 2 /nobreak >nul

echo 2. Displaying rotating strobing texts every 1.5 seconds for 8 seconds
dotnet run --project Overlay.Cli -- -texts "STROBE 1;STROBE 2;STROBE 3;STROBE 4" -duration 8 -switch 1.5 -foreground "#800080" -dropshadow "#FF69B4" -strobing -display 1 -column 0 -row 0

timeout /t 2 /nobreak >nul

echo 3. Displaying rotating texts on vertical display every 3 seconds for 12 seconds
dotnet run --project Overlay.Cli -- -texts "VERT 1;VERT 2;VERT 3" -duration 12 -switch 3 -foreground "#00FF00" -dropshadow "#000000" -display 2 -column 0 -row 1

echo.
echo All multiple text rotation tests completed!