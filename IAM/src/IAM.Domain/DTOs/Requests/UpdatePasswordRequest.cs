namespace IAM.Domain.DTOs.Requests;

public sealed record UpdatePasswordRequest
{
    public string Email { get; init; } = string.Empty;
    public string PasswordOld { get; init; } = string.Empty;
    public string PasswordNew { get; init; } = string.Empty;
}