namespace IAM.Domain.DTOs.Requests;

public sealed record UpdatePasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string PasswordOld { get; set; } = string.Empty;
    public string PasswordNew { get; set; } = string.Empty;
}