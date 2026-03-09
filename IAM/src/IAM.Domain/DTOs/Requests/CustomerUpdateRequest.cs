namespace IAM.Domain.DTOs.Requests;

public sealed record CustomerUpdateRequest
(
    string Name,
    string Code,
    string? Description,
    bool IsActive
);