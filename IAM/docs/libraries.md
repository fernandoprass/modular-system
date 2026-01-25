# Libraries in Use

## Overview
This project uses .NET 9.0 and leverages various NuGet packages to implement functionality efficiently. Libraries are chosen for their reliability, performance, and alignment with .NET ecosystem standards. Below is a summary of key libraries used across the project, grouped by layer or purpose.

## Core .NET Frameworks
- **Microsoft.NET.Sdk**: The base SDK for .NET projects, providing compilation and runtime support.
- **Microsoft.NET.Sdk.Web**: Extends the base SDK for web applications, including ASP.NET Core features.

## Web and API
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v9.0.0): Enables JWT (JSON Web Token) authentication in ASP.NET Core. JWT is a compact, URL-safe means of representing claims to be transferred between two parties, commonly used for secure API authentication.
- **Microsoft.AspNetCore.OpenApi** (v9.0.12): Provides OpenAPI/Swagger support for API documentation. OpenAPI is a specification for machine-readable interface files for describing, producing, consuming, and visualizing RESTful web services.
- **System.IdentityModel.Tokens.Jwt** (v8.0.1): Library for handling JWT tokens, including creation, validation, and parsing. Essential for implementing token-based security.

## Data Access and ORM
- **Microsoft.EntityFrameworkCore** (v9.0.0): The core Entity Framework Core package for ORM (Object-Relational Mapping). EF Core is Microsoft's modern ORM for .NET, enabling code-first database interactions with LINQ queries.
- **Microsoft.EntityFrameworkCore.Design** (v9.0.0): Tools for EF Core migrations and scaffolding. Used for generating database schemas from code models.
- **Microsoft.EntityFrameworkCore.Relational** (v9.0.0): Relational database provider for EF Core, supporting SQL databases.
- **Npgsql.EntityFrameworkCore.PostgreSQL** (v9.0.0): PostgreSQL provider for EF Core. PostgreSQL is an advanced open-source relational database known for its robustness and feature set.

## Business Logic and Utilities
- **BCrypt.Net-Next** (v4.0.3): A .NET implementation of the BCrypt password hashing function. BCrypt is a key derivation function designed to be slow and computationally expensive, making it resistant to brute-force attacks.
- **Microsoft.Extensions.Configuration** (v9.0.0): Provides configuration abstractions for .NET applications, allowing settings from various sources (e.g., JSON files, environment variables).

## Testing
- **xunit** (v2.9.2): A free, open-source, community-focused unit testing tool for .NET. xUnit is a modern testing framework emphasizing simplicity and extensibility.
- **Microsoft.NET.Test.Sdk** (v17.12.0): The MSBuild targets and properties for .NET test projects, enabling test discovery and execution.
- **xunit.runner.visualstudio** (v2.8.2): Runner for xUnit tests in Visual Studio, providing test exploration and execution.
- **coverlet.collector** (v6.0.2): Code coverage collector for .NET, integrating with xUnit to measure test coverage.

## Shared Library
The Shared module is a simple .NET library without external dependencies, designed for packaging as a NuGet package for cross-module reuse.

## Rationale for Choices
- **EF Core with PostgreSQL**: Chosen for its strong LINQ support, migrations, and compatibility with PostgreSQL, a popular open-source database.
- **JWT for Auth**: Industry-standard for stateless authentication in web APIs.
- **BCrypt for Passwords**: Provides secure hashing to protect user credentials.
- **xUnit for Testing**: Lightweight and extensible, fitting .NET testing best practices.
- **ASP.NET Core**: Microsoft's modern web framework for building scalable APIs.

All packages are kept up-to-date with .NET 9.0 compatibility to ensure security and performance.