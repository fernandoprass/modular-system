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

## 3. Create Database from scratch
If you want to create a database from scrath, delete all files from src/IAM.Infrastructure/Migrations and run the following command from the root 
solution directory to create the initial migration and apply it:
`dotnet ef migrations add InitialCreate --project src/IAM.Infrastructure --startup-project src/IAM.API -o Migrations
dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 4. Inspect migrations (optional)
To list existing migrations, run the following command from the root solution directory:
`dotnet ef migrations list --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 5. Apply migrations to update the database
Run the following command from the root solution directory to apply migrations:
`dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 6. Add new migrations after model changes
After making changes to your entity models, create a new migration with the following command from the root solution directory:
`dotnet ef migrations add IamDbContext --project src/IAM.Infrastructure --startup-project src/IAM.API -o Migrations`
Then apply the new migration:
`dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 8. Rollback to a previous migration (if needed)
To rollback to a previous migration, run the following command from the root solution directory, replacing YourMigrationName with the target migration:
`dotnet ef database update YourMigrationName --project src/IAM.Infrastructure --startup-project src/IAM.API`

## 7. Drop the database (if needed)
To drop the database, run the following command from the root solution directory:
`dotnet ef database drop --project src/IAM.Infrastructure --startup-project src/IAM.API`

## Troubleshooting
If the CLI picks the wrong environment (so appsettings.Development.json isn't used), set it inline:
`ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project src/IAM.Infrastructure --startup-project src/IAM.API`

- Make sure the DB user has rights to create a database/schema.
- If you see no pending migrations, confirm the migration files exist in src/IAM.Infrastructure/Migrations and that IamDbContext in IAM.Infrastructure is the one referenced by your migrations (snapshot shows it is).
- After migrations succeed, your development Program.cs seeder will run (you already call seeder.SeedAsync() when in Development).