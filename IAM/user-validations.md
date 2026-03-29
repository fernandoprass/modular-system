
# User Validations

The following errors represent the business and security constraints enforced during user management and authentication processes. While the core identity management logic is now structured to support tenant isolation, more granular controls such as **InvalidDomainError** (email domain white-listing) and **EmailAlreadyRegisteredInTenantError** (tenant-specific email uniqueness) are planned for future implementation to further harden the system’s governance.

### 1. **ForbiddenCustomerError**
-   **Description**: This occurs when an authenticated operator attempts to create or modify a resource belonging to a different `CustomerId` than their own. 
-   **Impact**: It prevents lateral privilege escalation (IDOR), ensuring a user from Company A cannot inject or edit data in Company B.

### 2. **UserNotFoundError**
-   **Description**: Raised when a requested operation (Update, Delete, or Password Change) targets a `UserId` or `Email` that does not exist in the database.
-   **Impact**: Prevents the application from attempting to process logic on null objects and provides a clear failure state for the UI.

### 3. **PasswordNotValidError**
-   **Description**: Used during login or password updates when the provided "old" or "current" password does not match the stored Argon2 hash.
-   **Impact**: Core security check that ensures identity verification before allowing sensitive credential changes.

### 4. **InactiveUserError**
-   **Description**: Occurs when a user successfully authenticates but their `IsActive` flag is set to `false`.
-   **Impact**: Allows administrators to revoke access (e.g., for former employees) without deleting the user's historical data or identity.

### 5. **InvalidDomainError**
-   **Description**: Triggered when a `Customer` has a whitelist of allowed email domains (e.g., `@company.com`) and the provided email address does not match.
-   **Impact**: Enforces corporate governance by preventing the use of personal or unauthorized email providers for business accounts.

### 6 **EmailAlreadyRegisteredInTenantError**
-   **Description**: Indicates that the email address is already associated with a user account within the specific context of the current `CustomerId`.
-   **Impact**: Supports multi-tenancy by allowing the same email to exist in the global database while ensuring uniqueness within each individual client.