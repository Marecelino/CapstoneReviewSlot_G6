@echo off
echo ========================================================
echo   Run All Services Locally (Fixed Ports)
echo ========================================================

echo [1/8] Checking SQL Server database...
timeout /t 2 /nobreak >nul

echo [2/8] Starting Identity Service...
start "Identity API (5044)" cmd /k "title Identity API && cd Services\Identity\Identity.Api && dotnet run --urls=http://localhost:5044"

echo Waiting 5 seconds...
timeout /t 5 /nobreak >nul

echo [3/8] Starting Session Service...
start "Session API (5181)" cmd /k "title Session API && cd Services\Session\Session.Api && dotnet run --urls=http://localhost:5181"

echo [4/8] Starting Availability Service...
start "Availability API (5068)" cmd /k "title Availability API && cd Services\Availability\Availability.Api && dotnet run --urls=http://localhost:5068"

echo [5/8] Starting Registration Service...
start "Registration API (5194)" cmd /k "title Registration API && cd Services\Registration\Registration.Api && dotnet run --urls=http://localhost:5194"

echo [6/8] Starting Assignment Service...
start "Assignment API (5139)" cmd /k "title Assignment API && cd Services\Assignment\Assignment.Api && dotnet run --urls=http://localhost:5139"

echo [7/8] Starting Report Service...
start "Report API (51317)" cmd /k "title Report API && cd Services\Report\Report.Api && dotnet run --urls=http://localhost:51317"

echo Waiting 5 seconds...
timeout /t 5 /nobreak >nul

echo [8/8] Starting API Gateway...
start "API Gateway (5000)" cmd /k "title API Gateway && cd Services\Gateway && dotnet run --urls=http://localhost:5000"

echo ========================================================
echo   All services have been launched!
echo ========================================================
pause
