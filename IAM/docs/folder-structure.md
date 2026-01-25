# Folder Structure

## Overview
This project follows a **modular architecture** based on **Clean Architecture** principles. Clean Architecture is a software design philosophy that emphasizes separation of concerns, making systems more testable, maintainable, and independent of external frameworks. The core idea is to organize code into layers where dependencies flow inward, from outer layers (like UI and infrastructure) to inner layers (like business logic and domain).

In a modular system, the application is divided into self-contained modules, each handling a specific business domain (e.g., IAM for Identity and Access Management). This promotes scalability and allows teams to work on different modules independently.

## Root Level Structure
- `create-module.ps1` and `create-module-shared.ps1`: Scripts to scaffold new modules or shared libraries.
- `IAM/`: The IAM (Identity and Access Management) module.
- `Shared/`: Common utilities packaged as NuGet packages for reuse across modules.

## IAM Module Structure
The IAM module is organized into the following layers:

### `src/`
Contains the source code, divided into projects:

- **`IAM.Domain/`**: The innermost layer containing domain entities, value objects, and interfaces. This layer is independent of any external frameworks and represents the core business rules and data structures.
  - `Customer.cs`, `User.cs`: Domain entities.
  - `DTOs/`: Data Transfer Objects for requests and responses.
  - `Repositories/`: Interfaces for data access (e.g., `IUserRepository`).
  - `QueryRepositories/`: Interfaces for read-only queries (e.g., `IUserQueryRepository`).

- **`IAM.Core/`**: Contains application logic, services, and use cases. This layer orchestrates business operations and depends only on the Domain layer.
  - `Services/`: Business services like `UserService`, `AuthService`.
  - `Orchestrators/`: (Currently empty) For complex workflows.

- **`IAM.Infrastructure/`**: Handles external concerns like databases, APIs, and third-party services. This layer implements interfaces defined in Domain and Core.
  - `IamDbContext.cs`: Entity Framework DbContext for database interactions.
  - `Migrations/`: Database migration files.
  - `Repositories/`: Implementations of repository interfaces.
  - `QueryRepositories/`: Implementations for query operations.

- **`IAM.API/`**: The outermost layer, containing the Web API controllers and configuration. This layer depends on all inner layers and exposes endpoints for client interactions.
  - `Controllers/`: API controllers like `UserController`, `CustomerController`.
  - `Program.cs`: Application entry point with ASP.NET Core setup.
  - `appsettings.json`: Configuration files.

### `tests/`
- **`IAM.Core.Tests/`**: Unit tests for the Core layer, ensuring business logic correctness.

### `docs/`
- Documentation files (this file and others).

## Dependency Flow
Dependencies flow inward:
- API → Core → Infrastructure → Domain
- Infrastructure implements Domain interfaces.
- Core uses Domain abstractions.
- API uses Core services.

This structure ensures that changes in outer layers (e.g., switching databases) don't affect inner layers, promoting flexibility and testability.

## Shared Module
- `src/Shared/`: Common utilities as a NuGet package.
- `tests/Shared.Tests/`: Tests for shared components.

This setup allows for easy extension with new modules following the same pattern.