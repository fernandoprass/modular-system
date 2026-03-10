namespace IAM.Domain.DTOs.Requests;

public sealed record CustomerCreateRequest
(
    CustomerType Type,
    string Name,
    string Code,
    string Description,
    CustomerUserCreateRequest User
);