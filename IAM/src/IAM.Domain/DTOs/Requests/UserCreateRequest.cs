namespace IAM.Domain.DTOs.Requests;

public sealed record UserCreateRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
}