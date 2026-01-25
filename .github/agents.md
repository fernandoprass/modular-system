# Copilot Instructions for Modular System

## Architecture Overview
This is a modular .NET system using Clean Architecture principles. Each business module (e.g., IAM) is self-contained with Domain, Core, Infrastructure, and API layers. The Shared module provides common utilities packaged as NuGet packages. This is a code-first approach without database-first scaffolding.

- **Domain**: Entities, value objects, interfaces (e.g., `IAM.Domain/`)
- **Core**: Application logic, services, use cases (e.g., `IAM.Core/`)
- **Infrastructure**: External concerns like databases, APIs (e.g., `IAM.Infrastructure/` with EF Core + PostgreSQL)
- **API**: Web API controllers, configuration (e.g., `IAM.API/`)
- **Shared**: Cross-module utilities, built as NuGet packages

Dependencies flow inward: API → Core → Infrastructure → Domain.

## Module Creation
Use `create-module.ps1` to scaffold new modules:
- Run `.\create-module.ps1` (edit `$MODULE_NAME` inside)
- Creates folder structure, .sln, projects with correct references
- Infrastructure includes Npgsql.EntityFrameworkCore.PostgreSQL

For Shared libraries: `create-module-shared.ps1` creates packagable projects with Moq/FluentAssertions for tests.

## Build and Test Workflows
- **Build**: `dotnet build` in module root (e.g., `IAM/`) or project folders
- **Test**: `dotnet test` in module root
- **Run API**: `dotnet run` in `src/Module.API/`
- **Debug**: Use VS Code debugger on Program.cs or test files

No custom scripts; standard .NET CLI commands.

## Coding Conventions
- **Naming**: PascalCase for classes, camelCase for variables
- **Nullable**: Enabled globally (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled for cleaner code
- **Project Structure**: `src/` for code,  `docs/` for md file documentation, `tests/` for xUnit tests
- **Dependencies**: Reference only inward layers; Shared via NuGet
- **EF Migrations**: Run `dotnet ef migrations add` in Infrastructure project
- **Data Access classes**: When creating data access classes, user Repository (folder Repositories) for INSERT, UPDATE, DELETE, GetById, and GetAll operations and QueryRepository (folder QueryRepositories) for all other search queries. If the task involves listing, filtering, or reports, create or use a QueryRepository returning DTOs with no-tracking enabled.
- **Use record types for DTOs** to ensure immutability and value-based equality.
- **New tables**: should implement IEntityTypeConfiguration


## Key Files
- `create-module.ps1`: Template for new business modules
- `IAM/src/IAM.API/Program.cs`: API entry point (minimal ASP.NET Core setup)
- `Shared/src/Shared/Shared.csproj`: Packaged library example
- `README.md`: Basic project info

When adding features, place domain models in Domain, business logic in Core, data access in Infrastructure, and endpoints in API. Use Shared for reusable components.

## Database Migrations~
IAM module uses Entity Framework Core with PostgreSQL.

