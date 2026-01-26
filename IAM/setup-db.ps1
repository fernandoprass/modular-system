# --- Configuration ---
$ENV_FILE = ".env"
$DOCKER_COMPOSE_FILE = "docker-compose.yml"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "PostgreSQL Environment Setup"
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Check if docker-compose.yml exists
if (-not (Test-Path $DOCKER_COMPOSE_FILE)) {
    Write-Host "[ERROR] $DOCKER_COMPOSE_FILE not found in this directory!" -ForegroundColor Red
    exit
}

# 2. Check if .env file exists, if not, create it with default values
if (-not (Test-Path $ENV_FILE)) {
    Write-Host "[INFO] .env file not found. Creating a new one with default credentials..." -ForegroundColor Yellow
    
    $defaultEnvContent = @"
# PostgreSQL Configuration
POSTGRES_USER=fernando_admin
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=modular_system_db

# pgAdmin Configuration
PGADMIN_EMAIL=admin@system.com
PGADMIN_PASSWORD=admin_secret_pass
"@
    $defaultEnvContent | Set-Content -Path $ENV_FILE
    Write-Host "[SUCCESS] .env file created." -ForegroundColor Green
}
else {
    Write-Host "[INFO] .env file already exists. Skipping creation." -ForegroundColor Gray
}

# 3. Verify if Docker Desktop is running
Write-Host "[INFO] Checking Docker status..." -ForegroundColor Gray
& docker info >$null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Docker Desktop is not running. Please start Docker and try again." -ForegroundColor Red
    exit
}

# 4. Start Docker Compose
Write-Host "[INFO] Starting containers..." -ForegroundColor Cyan
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "----------------------------------------------------------" -ForegroundColor Green
    Write-Host "  Database environment is UP and RUNNING!" -ForegroundColor Green
    Write-Host "----------------------------------------------------------" -ForegroundColor Green
    Write-Host "  PostgreSQL: localhost:5432"
    Write-Host "  pgAdmin:    http://localhost:8080"
    Write-Host "----------------------------------------------------------"
}
else {
    Write-Host "[ERROR] Failed to start Docker containers." -ForegroundColor Red
}