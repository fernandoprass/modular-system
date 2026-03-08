namespace IAM.Domain.DTOs.Requests;

public sealed record UserUpdatePasswordRequest(
   string Email,
   string PasswordOld,
   string PasswordNew
);