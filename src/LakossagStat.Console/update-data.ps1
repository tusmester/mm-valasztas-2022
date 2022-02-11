dotnet build
dotnet run

Write-Host "Copying data file to the static web folder..."
Copy-Item "./App_Data/lakossag-data.json" -Destination "../LakossagStat.StaticWeb/data"