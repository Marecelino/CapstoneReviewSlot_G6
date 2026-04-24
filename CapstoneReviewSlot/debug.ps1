$adminBody = @{ email = "admin@admin.com"; password = "Admin@123" }
$loginResp = Invoke-RestMethod -Method POST -Uri "http://localhost:5044/api/auth/login" -Body ($adminBody | ConvertTo-Json) -Headers @{ "Content-Type" = "application/json" }

$token = $loginResp.data.token

$campBody = @{
    name = "Test"
    startTime = "2026-04-20T00:00:00Z"
    endTime = "2026-04-25T00:00:00Z"
    maxGroupsPerLecturer = 5
    requiredReviewersPerGroup = 2
}

try {
    Invoke-RestMethod -Method POST -Uri "http://localhost:5181/api/campaigns" -Body ($campBody | ConvertTo-Json) -Headers @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
} catch {
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    $errorMsg = $reader.ReadToEnd()
    Write-Host "ERROR:"
    Write-Host $errorMsg
}
