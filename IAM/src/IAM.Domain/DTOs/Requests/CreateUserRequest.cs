namespace IAM.Domain.DTOs.Requests;

public sealed record CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
}