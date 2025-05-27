# Get all directories named 'TestResults' recursively
$folders = Get-ChildItem -Path . -Directory -Recurse -Filter TestResults

foreach ($folder in $folders) {
    try {
        Remove-Item -Path $folder.FullName -Recurse -Force
        Write-Host "Deleted folder: $($folder.FullName)"
    }
    catch {
        Write-Warning "Failed to delete folder: $($folder.FullName). Error: $_"
    }
}

# Roda os testes xunit
Get-ChildItem -Path ".\test" -Recurse -Filter "*.csproj" | ForEach-Object {
    dotnet test $_.FullName --collect:"XPlat Code Coverage"
}

# pega os resultados e gera um report só
reportgenerator -reports:($(Get-ChildItem -Recurse -Filter "coverage.cobertura.xml" -Path ".\test" | ForEach-Object { $_.FullName }) -join ";") -targetdir:coveragereport
