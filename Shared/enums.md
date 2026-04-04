
# Shared Enums

Enumerations in the Shared module define strongly-typed categories and states used across the domain. They ensure type safety, simplify database storage (using underlying numeric types), and dictate how data should be handled by application services and client-side applications.

## ParameterType (Enum)

The `ParameterType` enum defines the expected data format and semantic meaning of a `Parameter`'s value. Because parameters are stored as strings in the database, this enum instructs the system on how to parse, validate, and display the underlying data.

### Underlying Type

-   **`byte`**: This enum is explicitly backed by a `byte` (8-bit unsigned integer). This is an optimization choice to save database storage and memory, as the number of parameter types is small and well within the 0-255 range.
    

### Values

-   **Boolean (1)**: Represents a true/false value. In the UI, this typically renders as a toggle switch or checkbox.
-   **Integer (2)**: Represents whole numbers without fractions.
-   **Numeric (3)**: Represents decimal or floating-point numbers (e.g., currency, percentages, precise measurements).
-   **DateTime (4)**: Represents a specific date and time combination.
-   **Date (5)**: Represents a specific date (year, month, day) without a time component.
-   **Time (6)**: Represents a specific time of day independent of a date.
-   **Character (7)**: Represents a single alphanumeric character.
-   **String (8)**: Represents standard plain text. This is often used as the default type for general inputs.
-   **List (9)**: Represents a value that must be selected from a predefined static set of options. _Note: This works in conjunction with the `ListItems` property on the `Parameter` entity._
-   **ExternalList (10)**: Represents a value that must be selected from a dynamically fetched list. _Note: This works in conjunction with the `ExternalListEndpoint` property on the `Parameter` entity, prompting the UI to call an external API to get the selectable options._
    

### Theoretical & Architecture Context

This enumeration plays a crucial role in the **Dynamic UI rendering pattern**. When a front-end application (like a React or Angular portal) fetches a parameter, it reads the `Type` property to determine exactly which input component to render:

-   If `Type == ParameterType.Date`, render a DatePicker.
-   If `Type == ParameterType.List`, parse the `ListItems` JSON and render a Dropdown.
-   If `Type == ParameterType.Boolean`, render a Toggle.
    

Furthermore, the `ParameterService` uses this enum implicitly when exposing typed getter methods (like `GetIntAsync`, `GetBoolAsync`, `GetDecimalAsync`), ensuring the stored string value is safely converted back to its intended domain type.