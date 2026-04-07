namespace IAM.Domain.DTOs.Requests;

public sealed record CustomerUpdateRequest
(
    string Name,
    string? Description,
    bool IsActive
);