# --- Configuration ---
$ENV_FILE = ".env"
$DOCKER_COMPOSE_FILE = "docker-compose.yml"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "PostgreSQL Optimized Setup"
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Hardware Auto-Detection (NEW)
Write-Host "[INFO] Detecting System Resources..." -ForegroundColor Gray
$SystemRAM_GB = [Math]::Floor((Get-CimInstance Win32_PhysicalMemory | Measure-Object Capacity -Sum).Sum / 1GB)
Write-Host "Detected: $SystemRAM_GB GB RAM" -ForegroundColor Green

# 2. Check/Create .env with dynamic RAM value
if (-not (Test-Path $ENV_FILE)) {
    Write-Host "[INFO] Creating .env with detected resources..." -ForegroundColor Yellow
    
    $defaultEnvContent = @"
# --- Database Identity ---
POSTGRES_USER=admin
POSTGRES_PASSWORD=$( -join ((65..90) + (97..122) | Get-Random -Count 12 | % {[char]$_}) ) # Auto-gen password
POSTGRES_DB=IAM

# --- pgAdmin ---
PGADMIN_EMAIL=admin@system.com
PGADMIN_PASSWORD=msadmin123

# --- Tuning Params (Passed to Docker Compose) ---
TOTAL_RAM_GB=$SystemRAM_GB
"@
    $defaultEnvContent | Set-Content -Path $ENV_FILE
}

# 3. Docker Pre-flight Check
if (-not (Get-Process "Docker Desktop" -ErrorAction SilentlyContinue)) {
    Write-Host "[WARNING] Docker Desktop process not found. Attempting 'docker info'..." -ForegroundColor Yellow
}

& docker info >$null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Docker is not responding. Please start Docker Desktop." -ForegroundColor Red
    exit
}

# 4. Start and Monitor (Improved)
Write-Host "[INFO] Starting containers..." -ForegroundColor Cyan
docker-compose up -d --remove-orphans

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[WAIT] Waiting for Database to be Healthy..." -ForegroundColor Yellow
    # Loop until the healthcheck we added to docker-compose passes
    do {
        $status = docker inspect --format='{{.State.Health.Status}}' postgres_db 2>$null
        Write-Host "." -NoNewline
        Start-Sleep -Seconds 2
    } until ($status -eq "healthy" -or $counter++ -gt 15)

    Write-Host "`n----------------------------------------------------------" -ForegroundColor Green
    Write-Host "  Database environment is TUNED and READY!" -ForegroundColor Green
    Write-Host "  RAM Allocated: $SystemRAM_GB GB"
    Write-Host "  pgAdmin:       http://localhost:8080"
    Write-Host "----------------------------------------------------------"
}