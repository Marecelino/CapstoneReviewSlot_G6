$files = Get-ChildItem -Path Services -Recurse -Filter appsettings.Development.json
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $newContent = $content -replace 'Server=[^;]+;', 'Server=127.0.0.1,1433;'
    if ($content -ne $newContent) {
        Set-Content -Path $f.FullName -Value $newContent
        Write-Host "Updated $($f.FullName)"
    }
}
