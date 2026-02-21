namespace IAM.Domain.DTOs.Requests;

public sealed record UserUpdateRequest
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}