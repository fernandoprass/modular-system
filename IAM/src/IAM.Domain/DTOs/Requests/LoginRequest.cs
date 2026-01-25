namespace IAM.Domain.DTOs.Requests;

public sealed record LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}