@echo off
echo Displaying strobing purple text in column 0, row 0 (top left) on primary display
dotnet run --project Overlay.Cli -- -text "TOP LEFT BOX" -duration 10 -foreground "#800080" -dropshadow "#FF69B4" -strobing -display 1 -column 0 -row 0