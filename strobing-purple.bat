@echo off
echo Displaying strobing purple text with pink drop shadow on Display 2 (primary display) centered for 10 seconds
dotnet run --project Overlay.Cli -- -text "STROBING TEXT" -duration 10 -foreground "#800080" -dropshadow "#FF69B4" -strobing -display 1 -center