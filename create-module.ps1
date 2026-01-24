# --- Configuration ---
$MODULE_NAME = "IAM" # Change this for new modules
$DOTNET_VERSION = "net9.0"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Creating Business Module Structure"
Write-Host "Module: $MODULE_NAME"
Write-Host "==========================================" -ForegroundColor Cyan

# Create root folder
New-Item -ItemType Directory -Name $MODULE_NAME -Force | Out-Null
Set-Location $MODULE_NAME

# Create folders
New-Item -ItemType Directory -Path "src", "tests" -Force | Out-Null
dotnet new sln -n $MODULE_NAME

# 1. Domain
dotnet new classlib -n "$MODULE_NAME.Domain" -o "src/$MODULE_NAME.Domain" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Domain"

# 2. Core
dotnet new classlib -n "$MODULE_NAME.Core" -o "src/$MODULE_NAME.Core" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Core"
dotnet add "src/$MODULE_NAME.Core" reference "src/$MODULE_NAME.Domain"

# 3. Infrastructure
dotnet new classlib -n "$MODULE_NAME.Infrastructure" -o "src/$MODULE_NAME.Infrastructure" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.Infrastructure"
dotnet add "src/$MODULE_NAME.Infrastructure" reference "src/$MODULE_NAME.Domain"
dotnet add "src/$MODULE_NAME.Infrastructure" reference "src/$MODULE_NAME.Core"
dotnet add "src/$MODULE_NAME.Infrastructure" package Npgsql.EntityFrameworkCore.PostgreSQL

# 4. API
dotnet new webapi -n "$MODULE_NAME.API" -o "src/$MODULE_NAME.API" -f $DOTNET_VERSION
dotnet sln add "src/$MODULE_NAME.API"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Domain"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Core"
dotnet add "src/$MODULE_NAME.API" reference "src/$MODULE_NAME.Infrastructure"

# 5. Tests (Example: Core Tests)
dotnet new xunit -n "$MODULE_NAME.Core.Tests" -o "tests/$MODULE_NAME.Core.Tests" -f $DOTNET_VERSION
dotnet sln add "tests/$MODULE_NAME.Core.Tests"
dotnet add "tests/$MODULE_NAME.Core.Tests" reference "src/$MODULE_NAME.Core"

# Cleanup
Get-ChildItem -Recurse -Filter "Class1.cs" | Remove-Item
Get-ChildItem -Recurse -Filter "UnitTest1.cs" | Remove-Item

Write-Host "Module $MODULE_NAME created successfully!" -ForegroundColor Green
Set-Location ..