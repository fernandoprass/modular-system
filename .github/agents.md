# Copilot Instructions for Modular System

## Architecture Overview
This is a modular .NET system using Clean Architecture principles, it is a code-first approach without database-first scaffolding.

Each business module (Shared and IAM) is self-contained with Domain, Application, Infrastructure, and API layers. The Shared module provides common utilities and is referenced as a dependency in the IAM module. MD files are available at `docs/` for detailed documentation.

- **Domain**: Entities, value objects, interfaces (e.g., `IAM.Domain/`)
- **Application**: Application logic, services, use cases (e.g., `IAM.Application/`)
- **Infrastructure**: External concerns like databases, APIs (e.g., `IAM.Infrastructure/` with EF Core + PostgreSQL)
- **API**: Web API controllers, configuration (e.g., `IAM.API/`)
- **Shared**: Cross-module utilities, built as NuGet packages but it is referenced directly during development (e.g., `Shared/`)

Dependencies flow inward: API → Application → Infrastructure → Domain.

## Build and Test Workflows
- **Build**: `dotnet build` in module root (e.g., `IAM/`) or project folders
- **Test**: `dotnet test` in module root
- **Run API**: `dotnet run` in `src/02.IAM/IAM.API/`
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
- **Commenting**: Use XML comments for public members only for complex logic
- **Error Handling**: Use ErrorMessage for errors, return Result<T> for expected outcomes in Application layer
- **New Entities**: Place in Domain layer, use value objects where appropriate, and ensure they are properly encapsulated. Use as example: User.cs, UserService.cs, UserValidator.cs and UserRepository.cs in the IAM module.


## Key Files
- `infra/create-module.ps1`: Template for new business modules
- `src/01.Shared/Shared.Domain/Shared.Domain.csproj`: Packaged library example
- `src/02.IAM/IAM.API/Program.cs`: API entry point (minimal ASP.NET Core setup)
- `README.md`: Basic project info

When adding features, place domain models in Domain, business logic in Core, data access in Infrastructure, and endpoints in API. Use Shared for reusable components.

## Database Migrations
IAM module uses Entity Framework Core with PostgreSQL.

