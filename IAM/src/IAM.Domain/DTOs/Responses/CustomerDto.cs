namespace IAM.Domain.DTOs.Responses;

public sealed record CustomerDto
(
   Guid Id,
   CustomerType Type,
   string Code,
   string Name,
   string? Description,
   bool IsActive
);