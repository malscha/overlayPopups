Write-Host "Testing CLI mode of Overlay application" -ForegroundColor Green
Write-Host ""

Write-Host "Displaying 'Hello World' at position (200, 200) for 5 seconds" -ForegroundColor Yellow
dotnet run --project Overlay.Cli -- -x 200 -y 200 -text "Hello World" -duration 5

Start-Sleep -Seconds 2

Write-Host "Displaying 'CLI Test' at position (400, 400) for 3 seconds" -ForegroundColor Yellow
dotnet run --project Overlay.Cli -- -x 400 -y 400 -text "CLI Test" -duration 3

Start-Sleep -Seconds 2

Write-Host "Displaying 'PowerShell Overlay' at position (600, 200) for 4 seconds" -ForegroundColor Yellow
dotnet run --project Overlay.Cli -- -x 600 -y 200 -text "PowerShell Overlay" -duration 4

Write-Host ""
Write-Host "Test complete!" -ForegroundColor Green