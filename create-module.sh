#!/bin/bash

# Configuration Variables
MODULE_NAME="IAM"
DOTNET_VERSION="net10.0"  # Options: net6.0, net7.0, net8.0, net9.0

echo "=========================================="
echo "Creating Modular System Structure"
echo "Module: ${MODULE_NAME}"
echo ".NET Version: ${DOTNET_VERSION}"
echo "=========================================="

# Create root directory structure
mkdir -p src
mkdir -p tests

# Create solution file at root
echo "Creating solution..."
dotnet new sln -n ${MODULE_NAME}

# ============================================
# CREATE SHARED NUGET PROJECT
# ============================================
echo "Creating Shared NuGet project..."
dotnet new classlib -n Shared -o Shared -f ${DOTNET_VERSION}
dotnet sln add Shared

# Create Shared subdirectories
mkdir -p Shared/Extensions
mkdir -p Shared/Helpers
mkdir -p Shared/Constants
mkdir -p Shared/Models

# Remove default Class1.cs
rm Shared/Class1.cs

# Update Shared.csproj to be a NuGet package
cat > Shared/Shared.csproj << EOF
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>${DOTNET_VERSION}</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- NuGet Package Settings -->
    <PackageId>Shared</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Company>Your Company</Company>
    <Description>Shared utilities and helpers across all modules</Description>
    <PackageTags>shared;utilities;helpers</PackageTags>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>

</Project>
EOF

# ============================================
# CREATE MODULE PROJECTS (inside src folder)
# ============================================

# 1. Create Domain Layer
echo "Creating Domain layer..."
dotnet new classlib -n ${MODULE_NAME}.Domain -o src/${MODULE_NAME}.Domain -f ${DOTNET_VERSION}
dotnet sln add src/${MODULE_NAME}.Domain

# Create Domain subdirectories
mkdir -p src/${MODULE_NAME}.Domain/Entities
mkdir -p src/${MODULE_NAME}.Domain/ValueObjects
mkdir -p src/${MODULE_NAME}.Domain/Enums

# Remove default Class1.cs
rm src/${MODULE_NAME}.Domain/Class1.cs

# 2. Create Core Layer
echo "Creating Core/Business layer..."
dotnet new classlib -n ${MODULE_NAME}.Core -o src/${MODULE_NAME}.Core -f ${DOTNET_VERSION}
dotnet sln add src/${MODULE_NAME}.Core

# Add references
dotnet add src/${MODULE_NAME}.Core reference src/${MODULE_NAME}.Domain
dotnet add src/${MODULE_NAME}.Core reference Shared

# Create Core subdirectories
mkdir -p src/${MODULE_NAME}.Core/Interfaces
mkdir -p src/${MODULE_NAME}.Core/Services
mkdir -p src/${MODULE_NAME}.Core/Orchestrators
mkdir -p src/${MODULE_NAME}.Core/DTOs
mkdir -p src/${MODULE_NAME}.Core/Exceptions

# Remove default Class1.cs
rm src/${MODULE_NAME}.Core/Class1.cs

# 3. Create Infrastructure Layer
echo "Creating Infrastructure/Data Access layer..."
dotnet new classlib -n ${MODULE_NAME}.Infrastructure -o src/${MODULE_NAME}.Infrastructure -f ${DOTNET_VERSION}
dotnet sln add src/${MODULE_NAME}.Infrastructure

# Add references
dotnet add src/${MODULE_NAME}.Infrastructure reference src/${MODULE_NAME}.Domain
dotnet add src/${MODULE_NAME}.Infrastructure reference src/${MODULE_NAME}.Core
dotnet add src/${MODULE_NAME}.Infrastructure reference Shared

# Add EF Core packages
dotnet add src/${MODULE_NAME}.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/${MODULE_NAME}.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/${MODULE_NAME}.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL

# Create Infrastructure subdirectories
mkdir -p src/${MODULE_NAME}.Infrastructure/Data
mkdir -p src/${MODULE_NAME}.Infrastructure/Data/Configurations
mkdir -p src/${MODULE_NAME}.Infrastructure/Repositories
mkdir -p src/${MODULE_NAME}.Infrastructure/Services

# Remove default Class1.cs
rm src/${MODULE_NAME}.Infrastructure/Class1.cs

# 4. Create API Layer
echo "Creating API/Control layer..."
dotnet new webapi -n ${MODULE_NAME}.API -o src/${MODULE_NAME}.API -f ${DOTNET_VERSION}
dotnet sln add src/${MODULE_NAME}.API

# Add references
dotnet add src/${MODULE_NAME}.API reference src/${MODULE_NAME}.Domain
dotnet add src/${MODULE_NAME}.API reference src/${MODULE_NAME}.Core
dotnet add src/${MODULE_NAME}.API reference src/${MODULE_NAME}.Infrastructure
dotnet add src/${MODULE_NAME}.API reference Shared

# Add additional API packages
dotnet add src/${MODULE_NAME}.API package Microsoft.EntityFrameworkCore.Tools

# Create API subdirectories
mkdir -p src/${MODULE_NAME}.API/Controllers
mkdir -p src/${MODULE_NAME}.API/Middleware
mkdir -p src/${MODULE_NAME}.API/Filters

# Remove default WeatherForecast files if they exist
rm -f src/${MODULE_NAME}.API/WeatherForecast.cs
rm -f src/${MODULE_NAME}.API/Controllers/WeatherForecastController.cs

# ============================================
# CREATE TEST PROJECTS (inside tests folder)
# ============================================

# 5. Create Shared Tests
echo "Creating Shared test project..."
dotnet new xunit -n Shared.Tests -o tests/Shared.Tests -f ${DOTNET_VERSION}
dotnet sln add tests/Shared.Tests

dotnet add tests/Shared.Tests reference Shared

# Add testing packages
dotnet add tests/Shared.Tests package Moq
dotnet add tests/Shared.Tests package FluentAssertions

# Create test subdirectories
mkdir -p tests/Shared.Tests/Extensions
mkdir -p tests/Shared.Tests/Helpers
mkdir -p tests/Shared.Tests/Models

# Remove default UnitTest1.cs
rm -f tests/Shared.Tests/UnitTest1.cs

# 6. Create Domain Tests
echo "Creating Domain test project..."
dotnet new xunit -n ${MODULE_NAME}.Domain.Tests -o tests/${MODULE_NAME}.Domain.Tests -f ${DOTNET_VERSION}
dotnet sln add tests/${MODULE_NAME}.Domain.Tests

dotnet add tests/${MODULE_NAME}.Domain.Tests reference src/${MODULE_NAME}.Domain
dotnet add tests/${MODULE_NAME}.Domain.Tests reference Shared

# Add testing packages
dotnet add tests/${MODULE_NAME}.Domain.Tests package Moq
dotnet add tests/${MODULE_NAME}.Domain.Tests package FluentAssertions

# Create test subdirectories
mkdir -p tests/${MODULE_NAME}.Domain.Tests/Entities
mkdir -p tests/${MODULE_NAME}.Domain.Tests/ValueObjects

# Remove default UnitTest1.cs
rm -f tests/${MODULE_NAME}.Domain.Tests/UnitTest1.cs

# 7. Create Core Tests
echo "Creating Core test project..."
dotnet new xunit -n ${MODULE_NAME}.Core.Tests -o tests/${MODULE_NAME}.Core.Tests -f ${DOTNET_VERSION}
dotnet sln add tests/${MODULE_NAME}.Core.Tests

dotnet add tests/${MODULE_NAME}.Core.Tests reference src/${MODULE_NAME}.Domain
dotnet add tests/${MODULE_NAME}.Core.Tests reference src/${MODULE_NAME}.Core
dotnet add tests/${MODULE_NAME}.Core.Tests reference Shared

# Add testing packages
dotnet add tests/${MODULE_NAME}.Core.Tests package Moq
dotnet add tests/${MODULE_NAME}.Core.Tests package FluentAssertions

# Create test subdirectories
mkdir -p tests/${MODULE_NAME}.Core.Tests/Services
mkdir -p tests/${MODULE_NAME}.Core.Tests/Orchestrators
mkdir -p tests/${MODULE_NAME}.Core.Tests/Validators

# Remove default UnitTest1.cs
rm -f tests/${MODULE_NAME}.Core.Tests/UnitTest1.cs

# 8. Create Infrastructure Tests
echo "Creating Infrastructure test project..."
dotnet new xunit -n ${MODULE_NAME}.Infrastructure.Tests -o tests/${MODULE_NAME}.Infrastructure.Tests -f ${DOTNET_VERSION}
dotnet sln add tests/${MODULE_NAME}.Infrastructure.Tests

dotnet add tests/${MODULE_NAME}.Infrastructure.Tests reference src/${MODULE_NAME}.Domain
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests reference src/${MODULE_NAME}.Core
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests reference src/${MODULE_NAME}.Infrastructure
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests reference Shared

# Add testing packages
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests package Moq
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests package FluentAssertions
dotnet add tests/${MODULE_NAME}.Infrastructure.Tests package Microsoft.EntityFrameworkCore.InMemory

# Create test subdirectories
mkdir -p tests/${MODULE_NAME}.Infrastructure.Tests/Repositories
mkdir -p tests/${MODULE_NAME}.Infrastructure.Tests/Data

# Remove default UnitTest1.cs
rm -f tests/${MODULE_NAME}.Infrastructure.Tests/UnitTest1.cs

# 9. Create API Tests
echo "Creating API test project..."
dotnet new xunit -n ${MODULE_NAME}.API.Tests -o tests/${MODULE_NAME}.API.Tests -f ${DOTNET_VERSION}
dotnet sln add tests/${MODULE_NAME}.API.Tests

dotnet add tests/${MODULE_NAME}.API.Tests reference src/${MODULE_NAME}.Domain
dotnet add tests/${MODULE_NAME}.API.Tests reference src/${MODULE_NAME}.Core
dotnet add tests/${MODULE_NAME}.API.Tests reference src/${MODULE_NAME}.Infrastructure
dotnet add tests/${MODULE_NAME}.API.Tests reference src/${MODULE_NAME}.API
dotnet add tests/${MODULE_NAME}.API.Tests reference Shared

# Add testing packages
dotnet add tests/${MODULE_NAME}.API.Tests package Moq
dotnet add tests/${MODULE_NAME}.API.Tests package FluentAssertions
dotnet add tests/${MODULE_NAME}.API.Tests package Microsoft.AspNetCore.Mvc.Testing

# Create test subdirectories
mkdir -p tests/${MODULE_NAME}.API.Tests/Controllers
mkdir -p tests/${MODULE_NAME}.API.Tests/Integration

# Remove default UnitTest1.cs
rm -f tests/${MODULE_NAME}.API.Tests/UnitTest1.cs

# ============================================
# CREATE DOCUMENTATION
# ============================================

# Create README
echo "Creating README..."
cat > README.md << EOF
# ${MODULE_NAME}

## Project Structure

\`\`\`
.
├── ${MODULE_NAME}.sln
├── Shared/                                    # NuGet package - shared utilities
├── src/
│   ├── ${MODULE_NAME}.Domain/                 # Entities & Domain Models
│   ├── ${MODULE_NAME}.Core/                   # Business Logic & Interfaces
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Orchestrators/                     # Complex workflow coordination
│   │   ├── DTOs/
│   │   └── Exceptions/
│   ├── ${MODULE_NAME}.Infrastructure/         # Data Access & External Services
│   └── ${MODULE_NAME}.API/                    # Web API (Controllers)
└── tests/
    ├── Shared.Tests/                          # Shared utilities tests
    ├── ${MODULE_NAME}.Domain.Tests/           # Domain layer tests
    ├── ${MODULE_NAME}.Core.Tests/             # Business logic tests
    ├── ${MODULE_NAME}.Infrastructure.Tests/   # Data access tests
    └── ${MODULE_NAME}.API.Tests/              # API & Integration tests
\`\`\`

## Layer Dependencies

- **Domain**: No dependencies
- **Core**: References Domain, Shared
- **Infrastructure**: References Domain, Core, Shared
- **API**: References all layers + Shared
- **Tests**: Each test project references its corresponding project + dependencies
- **Shared**: NuGet package (no dependencies)

## Technology Stack

- .NET Version: ${DOTNET_VERSION}
- Database: PostgreSQL
- ORM: Entity Framework Core (Code-First)
- Testing: xUnit, Moq, FluentAssertions
- Shared: NuGet package for cross-module utilities

## Getting Started

### 1. Build the Shared NuGet package
\`\`\`bash
dotnet pack Shared -o ./nuget-packages
\`\`\`

### 2. Update connection string
Edit \`src/${MODULE_NAME}.API/appsettings.json\`:
\`\`\`json
"ConnectionStrings": {
  "${MODULE_NAME}Database": "Host=localhost;Database=${MODULE_NAME}DB;Username=postgres;Password=yourpassword"
}
\`\`\`

### 3. Run migrations
\`\`\`bash
dotnet ef migrations add InitialCreate --project src/${MODULE_NAME}.Infrastructure --startup-project src/${MODULE_NAME}.API
dotnet ef database update --project src/${MODULE_NAME}.Infrastructure --startup-project src/${MODULE_NAME}.API
\`\`\`

### 4. Run the API
\`\`\`bash
dotnet run --project src/${MODULE_NAME}.API
\`\`\`

### 5. Run tests
\`\`\`bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/${MODULE_NAME}.Core.Tests
dotnet test tests/${MODULE_NAME}.API.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
\`\`\`

## Building and Publishing

### Build entire solution
\`\`\`bash
dotnet build
\`\`\`

### Publish API
\`\`\`bash
dotnet publish src/${MODULE_NAME}.API -c Release -o ./publish
\`\`\`

### Pack Shared as NuGet
\`\`\`bash
dotnet pack Shared -c Release -o ./nuget-packages
\`\`\`

## Testing Structure

Each layer has its own test project:
- **Shared.Tests**: Shared utilities, extensions, helpers
- **Domain.Tests**: Entity validation, value objects, domain logic
- **Core.Tests**: Business rules, services, orchestrators, DTOs
- **Infrastructure.Tests**: Repository patterns, database operations
- **API.Tests**: Controllers, middleware, integration tests

## Development Workflow

1. Define entities in \`${MODULE_NAME}.Domain/Entities\`
2. Create interfaces in \`${MODULE_NAME}.Core/Interfaces\`
3. Implement business logic in \`${MODULE_NAME}.Core/Services\`
4. Implement data access in \`${MODULE_NAME}.Infrastructure/Repositories\`
5. Create controllers in \`${MODULE_NAME}.API/Controllers\`
6. Write tests for each layer in corresponding test projects

## Configuration

To customize the generation:
- \`MODULE_NAME\`: Change for different modules (e.g., 'Billing', 'Inventory')
- \`DOTNET_VERSION\`: Change target framework (e.g., 'net6.0', 'net7.0', 'net8.0', 'net9.0')
EOF

# Create .gitignore
echo "Creating .gitignore..."
cat > .gitignore << 'EOF'
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/
publish/

# Visual Studio cache/options
.vs/
.vscode/

# Test Results
TestResults/
*.trx
*.coverage
*.coveragexml

# NuGet Packages
*.nupkg
**/packages/*
nuget-packages/

# Database
*.db
*.db-shm
*.db-wal

# EF Core Migrations (uncomment if you want to ignore)
# **/Migrations/

# Rider
.idea/
EOF

echo "=========================================="
echo "✓ Solution structure created successfully!"
echo "=========================================="
echo ""
echo "Structure created:"
echo "  ├── ${MODULE_NAME}.sln"
echo "  ├── Shared/                       (NuGet package)"
echo "  ├── src/"
echo "  │   ├── ${MODULE_NAME}.Domain/"
echo "  │   ├── ${MODULE_NAME}.Core/"
echo "  │   ├── ${MODULE_NAME}.Infrastructure/"
echo "  │   └── ${MODULE_NAME}.API/"
echo "  └── tests/"
echo "      ├── Shared.Tests/"
echo "      ├── ${MODULE_NAME}.Domain.Tests/"
echo "      ├── ${MODULE_NAME}.Core.Tests/"
echo "      ├── ${MODULE_NAME}.Infrastructure.Tests/"
echo "      └── ${MODULE_NAME}.API.Tests/"
echo ""
echo "Configuration:"
echo "  .NET Version: ${DOTNET_VERSION}"
echo "  Testing: xUnit + Moq + FluentAssertions"
echo "  Shared: NuGet package"
echo ""
echo "Next steps:"
echo "1. Build Shared NuGet: dotnet pack Shared -o ./nuget-packages"
echo "2. Update appsettings.json in src/${MODULE_NAME}.API"
echo "3. Create entities in src/${MODULE_NAME}.Domain/Entities"
echo "4. Create DbContext in src/${MODULE_NAME}.Infrastructure/Data"
echo "5. Write tests in tests/ folder"
echo "6. Run: dotnet build && dotnet test"
echo ""
echo "To customize:"
echo "  - MODULE_NAME: Different modules (e.g., 'Billing', 'Inventory')"
echo "  - DOTNET_VERSION: Target framework (e.g., 'net8.0', 'net9.0')"