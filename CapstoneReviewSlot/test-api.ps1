# =============================================================
#  CapstoneReviewSlot - API Test Script (End-To-End)
# =============================================================

$IDENTITY     = "http://localhost:5044"
$SESSION      = "http://localhost:5181"
$AVAILABILITY = "http://localhost:5068"
$REGISTRATION = "http://localhost:5194"
$ASSIGNMENT   = "http://localhost:5139"
$REPORT       = "http://localhost:51317"

$FILE_GROUPS = "d:\22-4\CapstoneReviewSlot_G6\SE_CapstoneProject_FA25_Review_FIXED.xlsx"
$FILE_AVAIL  = "d:\22-4\CapstoneReviewSlot_G6\LecturerBookingReviewSlots_RegistrationForm_FIXED.xlsx"

$global:TOKEN_ADMIN    = ""
$global:TOKEN_LECTURER = ""
$global:CAMPAIGN_ID    = ""
$global:SLOT_ID        = ""
$global:GROUP_ID       = ""
$global:LECTURER_ID    = ""

$global:pass = 0
$global:fail = 0

function Write-Step($msg) { Write-Host "`n---- $msg ----" -ForegroundColor Cyan }
function Write-OK($msg)   { Write-Host "  [OK]   $msg" -ForegroundColor Green;  $global:pass++ }
function Write-FAIL($msg) { Write-Host "  [FAIL] $msg" -ForegroundColor Red;   $global:fail++ }
function Write-INFO($msg) { Write-Host "         $msg" -ForegroundColor Gray }

function Invoke-Api {
    param(
        [string]$Method = "GET",
        [string]$Url,
        [object]$Body = $null,
        [string]$Token = "",
        [string]$Label
    )
    $headers = @{ "Content-Type" = "application/json; charset=utf-8" }
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    try {
        $params = @{ Method = $Method; Uri = $Url; Headers = $headers; TimeoutSec = 30 }
        if ($Body) { 
            $jsonStr = $Body | ConvertTo-Json -Depth 10
            $params["Body"] = [System.Text.Encoding]::UTF8.GetBytes($jsonStr)
        }
        
        $resp = Invoke-WebRequest @params -UseBasicParsing -ErrorAction Stop
        $json = $resp.Content | ConvertFrom-Json -ErrorAction SilentlyContinue
        Write-OK "$Label -> HTTP $($resp.StatusCode)"
        return $json
    } catch {
        $code = "ERR"
        if ($_.Exception.Response) {
            $code = $_.Exception.Response.StatusCode.value__
        }
        Write-FAIL "$Label -> $code  ($($_.Exception.Message))"
        return $null
    }
}

function Invoke-FileUpload {
    param([string]$Url, [string]$FilePath, [string]$Token, [string]$Label)
    
    try {
        Add-Type -AssemblyName System.Net.Http
        $client = New-Object System.Net.Http.HttpClient
        $client.Timeout = [TimeSpan]::FromMinutes(2)
        $client.DefaultRequestHeaders.Authorization = New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $Token)

        $content = New-Object System.Net.Http.MultipartFormDataContent
        $fileStream = [System.IO.File]::OpenRead($FilePath)
        $streamContent = New-Object System.Net.Http.StreamContent($fileStream)
        
        $content.Add($streamContent, "file", [System.IO.Path]::GetFileName($FilePath))

        Write-INFO "Uploading $($FilePath)..."
        $response = $client.PostAsync($Url, $content).GetAwaiter().GetResult()
        $responseString = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        
        $fileStream.Close()
        $client.Dispose()
        
        $statusCode = [int]$response.StatusCode
        if ($response.IsSuccessStatusCode) {
            Write-OK "$Label -> HTTP $statusCode"
            return $responseString | ConvertFrom-Json
        } else {
            Write-FAIL "$Label -> HTTP $statusCode"
            Write-INFO $responseString
            return $null
        }
    } catch {
        Write-FAIL "$Label -> Error: $_"
        return $null
    }
}

# ---------------------------------------------
# STEP 1: IDENTITY - LOGIN ADMIN & LECTURER
# ---------------------------------------------
Write-Step "STEP 1: LOGIN (IDENTITY)"

$adminBody = @{ email = "admin@admin.com"; password = "Admin@123" }
$adminResp = Invoke-Api -Method POST -Url "$IDENTITY/api/auth/login" -Body $adminBody -Label "Login Manager"
if ($adminResp -and $adminResp.data.token) {
    $global:TOKEN_ADMIN = $adminResp.data.token
    Write-INFO "Token Manager OK!"
}

$lecBody = @{ email = "nguyentrongtai@capstone.test"; password = "Lecturer@123" }
$lecResp = Invoke-Api -Method POST -Url "$IDENTITY/api/auth/login" -Body $lecBody -Label "Login Lecturer"
if ($lecResp -and $lecResp.data.token) {
    $global:TOKEN_LECTURER = $lecResp.data.token
    $global:LECTURER_ID = $lecResp.data.lecturerId
    Write-INFO "Token Lecturer OK!"
}

# ---------------------------------------------
# STEP 2: CAMPAIGN (SESSION)
# ---------------------------------------------
Write-Step "STEP 2: CREATE CAMPAIGN"

$campBody = @{
    name = "Capstone Review SU2026 - Test Script"
    startTime = "2026-04-20T00:00:00.000Z"
    endTime = "2026-04-25T00:00:00.000Z"
    maxGroupsPerLecturer = 5
    requiredReviewersPerGroup = 2
}
$campResp = Invoke-Api -Method POST -Url "$SESSION/api/campaigns" -Body $campBody -Token $global:TOKEN_ADMIN -Label "Create Campaign"

if ($campResp -and $campResp.campaignId) {
    $global:CAMPAIGN_ID = $campResp.campaignId
    Write-INFO "Campaign ID: $($global:CAMPAIGN_ID)"
    
    $slotsBody = @{
        campaignId = $global:CAMPAIGN_ID
        slots = @(
            @{ date="2026-04-20"; startTime="08:00"; endTime="10:00"; room="Room A" },
            @{ date="2026-04-20"; startTime="10:00"; endTime="12:00"; room="Room B" },
            @{ date="2026-04-21"; startTime="08:00"; endTime="10:00"; room="Room C" }
        )
    }
    $slotsResp = Invoke-Api -Method POST -Url "$SESSION/api/campaigns/$($global:CAMPAIGN_ID)/slots" -Body $slotsBody -Token $global:TOKEN_ADMIN -Label "Create Slots"
    if ($slotsResp -and $slotsResp.Count -gt 0) {
        $global:SLOT_ID = $slotsResp[0].id
        Write-INFO "Slot ID: $($global:SLOT_ID)"
    }
}

# ---------------------------------------------
# STEP 3: IMPORT EXCEL (SESSION)
# ---------------------------------------------
Write-Step "STEP 3: IMPORT EXCEL"

if ($global:CAMPAIGN_ID) {
    $importUrl = "$SESSION/api/groups/campaign/$($global:CAMPAIGN_ID)/import"
    $grpResp = Invoke-FileUpload -Url $importUrl -FilePath $FILE_GROUPS -Token $global:TOKEN_ADMIN -Label "Import Groups Excel"
    if ($grpResp -and $grpResp.isSuccess) {
        Write-INFO "Import Groups Success: $($grpResp.value.successCount) groups."
        
        $allGroups = Invoke-Api -Method GET -Url "$SESSION/api/groups/campaign/$($global:CAMPAIGN_ID)" -Label "Get Imported Groups"
        if ($allGroups -and $allGroups.Count -gt 0) {
            $global:GROUP_ID = $allGroups[0].id
            Write-INFO "Group ID: $($global:GROUP_ID)"
        }
    }

    $availUrl = "$SESSION/api/groups/availability/import"
    $avResp = Invoke-FileUpload -Url $availUrl -FilePath $FILE_AVAIL -Token $global:TOKEN_ADMIN -Label "Import Availability Excel"
}

# ---------------------------------------------
# STEP 4: AVAILABILITY
# ---------------------------------------------
Write-Step "STEP 4: LECTURER AVAILABILITY"

if ($global:SLOT_ID -and $global:TOKEN_LECTURER) {
    $avReq = @{
        reviewSlotId = $global:SLOT_ID
        preferenceLevel = "Preferred"
    }
    Invoke-Api -Method POST -Url "$AVAILABILITY/api/availability/register" -Body $avReq -Token $global:TOKEN_LECTURER -Label "Register Slot (Preferred)" | Out-Null
    Invoke-Api -Method GET -Url "$AVAILABILITY/api/availability/my" -Token $global:TOKEN_LECTURER -Label "Get My Slots" | Out-Null
}

# ---------------------------------------------
# STEP 5: REGISTRATION
# ---------------------------------------------
Write-Step "STEP 5: STUDENT REGISTRATION"

if ($global:GROUP_ID -and $global:SLOT_ID) {
    $prefBody = @{
        capstoneGroupId = $global:GROUP_ID
        reviewSlotId    = $global:SLOT_ID
        preferenceLevel = "Preferred"
    }
    Invoke-Api -Method POST -Url "$REGISTRATION/api/preferences" -Body $prefBody -Token $global:TOKEN_ADMIN -Label "Student Registers Slot" | Out-Null
}

# ---------------------------------------------
# STEP 6: ASSIGNMENT
# ---------------------------------------------
Write-Step "STEP 6: MANAGER ASSIGNMENT"

if ($global:CAMPAIGN_ID) {
    Invoke-Api -Method GET -Url "$ASSIGNMENT/api/assignment/campaign/$($global:CAMPAIGN_ID)/summary" -Token $global:TOKEN_ADMIN -Label "Get Summary" | Out-Null
}

if ($global:GROUP_ID -and $global:SLOT_ID) {
    $rev1 = [Guid]::NewGuid().ToString()
    $rev2 = [Guid]::NewGuid().ToString()

    $assignBody = @{
        capstoneGroupId = $global:GROUP_ID
        reviewSlotId    = $global:SLOT_ID
        reviewerIds     = @( $rev1, $rev2 )
    }
    Invoke-Api -Method POST -Url "$ASSIGNMENT/api/assignment/manual-assign" -Body $assignBody -Token $global:TOKEN_ADMIN -Label "Manual Assign" | Out-Null
    Invoke-Api -Method GET -Url "$ASSIGNMENT/api/assignment/conflicts" -Token $global:TOKEN_ADMIN -Label "Check Conflicts" | Out-Null
}

# ---------------------------------------------
# STEP 7: REPORT
# ---------------------------------------------
Write-Step "STEP 7: REPORT"

if ($global:CAMPAIGN_ID) {
    Invoke-Api -Method GET -Url "$REPORT/api/reports/timeline/$($global:CAMPAIGN_ID)" -Token $global:TOKEN_ADMIN -Label "Get Timeline" | Out-Null
    Invoke-Api -Method GET -Url "$REPORT/api/reports/workload/$($global:CAMPAIGN_ID)" -Token $global:TOKEN_ADMIN -Label "Get Workload" | Out-Null
}

# ---------------------------------------------
# SUMMARY
# ---------------------------------------------
$color = "Yellow"
if ($global:fail -eq 0) { $color = "Green" }

Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "  API TEST SUMMARY" -ForegroundColor Yellow
Write-Host "  PASS: $($global:pass)  |  FAIL: $($global:fail)" -ForegroundColor $color
Write-Host "========================================`n" -ForegroundColor Yellow
