namespace IAM.Domain.DTOs
{
   public sealed record UserPasswordDto
   {
      public Guid Id { get; init; }
      public string Name { get; init; } = string.Empty;
      public string Email { get; init; } = string.Empty;
      public Guid CustomerId { get; init; }
      public string CustomerName { get; init; } = string.Empty;
      public string PasswordHash { get; init; } = string.Empty;
   }
}
