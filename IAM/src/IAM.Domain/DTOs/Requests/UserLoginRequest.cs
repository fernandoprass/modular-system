namespace IAM.Domain.DTOs.Requests;

public sealed record UserLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}