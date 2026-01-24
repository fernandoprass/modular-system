
# --- Configuration ---
$PROJECT_NAME = "Shared"
$DOTNET_VERSION = "net9.0"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Creating Standalone Shared Solution"
Write-Host "Project: $PROJECT_NAME"
Write-Host ".NET Version: $DOTNET_VERSION"
Write-Host "==========================================" -ForegroundColor Cyan

# Create root folder
New-Item -ItemType Directory -Name $PROJECT_NAME -Force | Out-Null
Set-Location $PROJECT_NAME

# Create folder structure
New-Item -ItemType Directory -Path "src", "tests" -Force | Out-Null

# Create Solution
dotnet new sln -n "$PROJECT_NAME"

# Create Library Project
dotnet new classlib -n "$PROJECT_NAME" -o "src/$PROJECT_NAME" -f $DOTNET_VERSION
dotnet sln add "src/$PROJECT_NAME"

# Create Test Project
dotnet new xunit -n "$PROJECT_NAME.Tests" -o "tests/$PROJECT_NAME.Tests" -f $DOTNET_VERSION
dotnet sln add "tests/$PROJECT_NAME.Tests"

# Add Test Dependencies
dotnet add "tests/$PROJECT_NAME.Tests" reference "src/$PROJECT_NAME"
dotnet add "tests/$PROJECT_NAME.Tests" package Moq
dotnet add "tests/$PROJECT_NAME.Tests" package FluentAssertions

# Cleanup default files
Remove-Item "src/$PROJECT_NAME/Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "tests/$PROJECT_NAME.Tests/UnitTest1.cs" -ErrorAction SilentlyContinue

# Configure NuGet Settings in .csproj
$csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>$DOTNET_VERSION</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PackageId>$PROJECT_NAME</PackageId>
    <Version>1.0.0</Version>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <Authors>YourCompany</Authors>
  </PropertyGroup>
</Project>
"@
$csprojContent | Set-Content -Path "src/$PROJECT_NAME/$PROJECT_NAME.csproj"

Write-Host "Shared Solution created successfully!" -ForegroundColor Green
Set-Location ..