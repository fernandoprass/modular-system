# Migration Guide

This guide outlines the steps to manage database migrations for the IAM.API project using Entity Framework Core.

## 1. Verify connection string
Ensure src/IAM.API/appsettings.json (or appsettings.Development.json) contains the connection under ConnectionStrings:IAM.
`{
  "ConnectionStrings": {
    "IAM": "Host=localhost;Database=IAM;Username=postgres;Password=yourpassword"
  }
}`

## 2. Ensure EF tooling & design package are available
Install CLI tooling if you don't have it:
`dotnet tool install --global dotnet-ef`

## 3. Inspect migrations (optional)
To list existing migrations, run the following command from the root solution directory:
`dotnet ef migrations list --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 4. Apply migrations to create/update the database
Run the following command from the root solution directory to apply migrations:
`dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

## Troubleshooting
If the CLI picks the wrong environment (so appsettings.Development.json isn't used), set it inline:
`ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

- Make sure the DB user has rights to create a database/schema.
- If you see no pending migrations, confirm the migration files exist in src/IAM.Infrastructure/Migrations and that IamDbContext in IAM.Infrastructure is the one referenced by your migrations (snapshot shows it is).
- After migrations succeed, your development Program.cs seeder will run (you already call seeder.SeedAsync() when in Development).