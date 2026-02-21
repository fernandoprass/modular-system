# Entities

## Overview
In Domain-Driven Design (DDD), entities are objects that have a distinct identity and lifecycle, representing core business concepts. Unlike value objects, entities are mutable and tracked by their unique identifier. In this project, entities are defined in the Domain layer, free from infrastructure concerns, ensuring business logic purity.

The IAM (Identity and Access Management) module manages users and customers, modeled as entities with relationships.

## User Entity
The `User` entity represents an individual user in the system, typically an employee or account holder associated with a customer.

### Properties
- **Id** (Guid): Unique identifier for the user. GUIDs ensure global uniqueness across distributed systems.
- **Name** (string): The user's full name.
- **Email** (string): The user's email address, used for login and communication.
- **PasswordHash** (string): Hashed version of the user's password for security. Passwords are never stored in plain text; instead, they are hashed using Isopoh.Cryptography.Argon2.
- **IsActive** (bool): Informs if the user is active.
- **CustomerId** (Guid): Foreign key linking the user to their associated customer.
- **CreatedAt** (DateTime): Timestamp of when the user was created, in UTC.
- **UpdatedAt** (DateTime?): Optional timestamp of the last update, in UTC.
- **EmailVerifiedAt** (DateTime) : Informs when the email was confirmed, if is null the email was not confirmed.
- **LastLoginAt** (Datetime): Timestamp of the last login
- **Customer** (navigation property): Reference to the associated `Customer` entity.

### Theoretical Context
Users are central to IAM systems, controlling access to resources. The entity includes authentication data (email, password) and audit fields (timestamps) for compliance. The relationship with Customer enables multi-tenancy, where users belong to organizations.

## Customer Entity
The `Customer` entity represents an organization or client that owns users and resources.

### Properties
- **Id** (Guid): Unique identifier for the customer.
- **Code** (string): Unique code representing the customer, often used for external reference.
- **Types (Enum CustomerType)**: The type of customer (1- Company, 2 - Person)
- **Name** (string): The customer's name (e.g., company name).
- **Description** (string?): Optional description of the customer.
- **IsActive** (bool): Informs if the customer is active.
- **CreatedAt** (DateTime): Timestamp of creation, in UTC.
- **UpdatedAt** (DateTime?): Optional timestamp of the last update, in UTC.
- **Users** (ICollection<User>): Collection of users belonging to this customer.

### Theoretical Context
In B2B applications, customers often represent tenants. This entity supports hierarchical data models, where customers contain users. The one-to-many relationship (Customer to Users) is a common pattern in relational databases, enforced via foreign keys.

## Relationships
- **User to Customer**: Many-to-one relationship. Each user belongs to one customer; a customer can have multiple users. This is implemented with `CustomerId` in User and navigation properties on both sides.

## Design Principles
- **Immutability of Identity**: The `Id` is set once and never changed.
- **Audit Trail**: `CreatedAt` and `UpdatedAt` provide traceability.
- **Security**: Passwords are hashed and salted to protect against breaches.
- **Nullability**: Enabled globally (`<Nullable>enable</Nullable>`), with nullable reference types for optional fields.
- **EF Core Integration**: Entities are designed for Entity Framework Core, with navigation properties for eager/lazy loading.

These entities form the foundation of the domain model, driving business rules and data persistence.