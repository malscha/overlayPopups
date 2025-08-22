@echo off
echo Displaying strobing purple text with pink drop shadow on Display 3 (left vertical display) centered for 10 seconds
dotnet run --project Overlay.Cli -- -text "VERTICAL DISPLAY" -duration 10 -foreground "#800080" -dropshadow "#FF69B4" -strobing -display 2 -center