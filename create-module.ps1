# --- Configuration ---
$MODULE_NAME = "IAM" # Change this for new modules
$DOTNET_VERSION = "net10.0"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Creating Business Module Structure"
Write-Host "Module: $MODULE_NAME"
Write-Host "==========================================" -ForegroundColor Cyan

# Create root folder
New-Item -ItemType Directory -Name $MODULE_NAME -Force | Out-Null
Set-Location $MODULE_NAME

# Create folders
New-Item -ItemType Directory -Path "src", "docs", "tests" -Force | Out-Null
dotnet new sln -n $MODULE_NAME

# 1. Domain
dotnet new classlib -n "$MODULE_NAME.Domain" -o "src/$MODULE_NAME.Domain" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Domain"

# Create Split Repository Structure
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Domain/DTOs" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Domain/Entities" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Domain/Interfaces" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Domain/QueryRepositories" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Domain/Repositories" -Force | Out-Null

# 2. Application
dotnet new classlib -n "$MODULE_NAME.Application" -o "src/$MODULE_NAME.Application" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Application"
dotnet add "src/$MODULE_NAME.Application" reference "src/$MODULE_NAME.Domain"

# Create Split Repository Structure
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Application/Contracts" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Application/Orchestrators" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Application/Services" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Application/Validators" -Force | Out-Null

# 3. Infrastructure
dotnet new classlib -n "$MODULE_NAME.Infrastructure" -o "src/$MODULE_NAME.Infrastructure" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Infrastructure"
dotnet add "src/$MODULE_NAME.Infrastructure" reference "src/$MODULE_NAME.Domain"
dotnet add "src/$MODULE_NAME.Infrastructure" reference "src/$MODULE_NAME.Application"
dotnet add "src/$MODULE_NAME.Infrastructure" package Npgsql.EntityFrameworkCore.PostgreSQL

# Create Split Repository Structure
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Infrastructure/Configurations" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Infrastructure/Migrations" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Infrastructure/QuertRepositories" -Force | Out-Null
New-Item -ItemType Directory -Path "src/$MODULE_NAME.Infrastructure/Repositories" -Force | Out-Null

# 4. API
dotnet new webapi -n "$MODULE_NAME.API" -o "src/$MODULE_NAME.API" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.API"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Domain"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Application"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Infrastructure"

# 5. Tests (Example: Application Tests)
dotnet new xunit -n "$MODULE_NAME.Application.Tests" -o "tests/$MODULE_NAME.Application.Tests" -f $DOTNET_VERSION
dotnet sln add "tests/$MODULE_NAME.Application.Tests"
dotnet add "tests/$MODULE_NAME.Application.Tests" reference "src/$MODULE_NAME.Application"

# Cleanup
Get-ChildItem -Recurse -Filter "Class1.cs" | Remove-Item
Get-ChildItem -Recurse -Filter "UnitTest1.cs" | Remove-Item

Write-Host "Module $MODULE_NAME created successfully!" -ForegroundColor Green
Set-Location ..