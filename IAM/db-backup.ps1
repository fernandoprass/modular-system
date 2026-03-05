# --- Configuration ---
$CONTAINER_NAME = "postgres_db"  # Must match your docker-compose container_name
$BACKUP_DIR = "C:\backup"
$DATE = Get-Date -Format "yyyy-MM-dd_HHmm"
$FILENAME = "IAM_backup_$DATE.sql.gz"
$FULL_PATH = Join-Path $BACKUP_DIR $FILENAME

# Load .env to get credentials
if (Test-Path ".env") {
    Get-Content ".env" | Where-Object { $_ -match "=" } | ForEach-Object {
        $name, $value = $_.Split('=', 2)
        Set-Variable -Name "ENV_$name" -Value $value -Scope Script
    }
}

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "PostgreSQL Docker Backup Routine"
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Create backup directory if it doesn't exist
if (-not (Test-Path $BACKUP_DIR)) {
    New-Item -ItemType Directory -Path $BACKUP_DIR | Out-Null
    Write-Host "[INFO] Created directory $BACKUP_DIR" -ForegroundColor Gray
}

# 2. Verify Container is running
$containerStatus = docker inspect -f '{{.State.Running}}' $CONTAINER_NAME 2>$null
if ($containerStatus -ne "true") {
    Write-Host "[ERROR] Container $CONTAINER_NAME is not running. Cannot backup." -ForegroundColor Red
    exit
}

# 3. Execute pg_dump and compress
Write-Host "[INFO] Starting backup of database: $($ENV_POSTGRES_DB)..." -ForegroundColor Cyan

# We use 'docker exec' to run pg_dump inside, and redirect output to a file on Windows
# -Fc = Custom format (compressed and flexible for restores)
& docker exec -t $CONTAINER_NAME pg_dump -U $ENV_POSTGRES_USER -Fc $ENV_POSTGRES_DB > $FULL_PATH

if ($LASTEXITCODE -eq 0) {
    $size = (Get-Item $FULL_PATH).Length / 1MB
    Write-Host "[SUCCESS] Backup saved to: $FULL_PATH" -ForegroundColor Green
    Write-Host "[INFO] Backup size: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
} else {
    Write-Host "[ERROR] Backup failed!" -ForegroundColor Red
    exit
}

# 4. Cleanup (Optional: Keep only last 7 days of backups)
Write-Host "[INFO] Cleaning up backups older than 7 days..." -ForegroundColor Gray
Get-ChildItem $BACKUP_DIR -Filter "backup_*.sql.gz" | 
    Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-7) } | 
    Remove-Item

Write-Host "Done." -ForegroundColor Cyan


####################################################################
########################## How to Restore ##########################
# To restore one of these backups to the Docker container, use this command in your terminal:
# Syntax: cat <backup_file> | docker exec -i <container_name> pg_restore -U <user> -d <db_name> -c
# cat C:\backup\backup_your_file.sql.gz | docker exec -i postgres_db pg_restore -U admin -d IAM -c --clean