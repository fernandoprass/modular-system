namespace IAM.Domain.DTOs
{
   public sealed record UserPasswordDto
   {
      public Guid Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public Guid CustomerId { get; set; }
      public string CustomerName { get; set; } = string.Empty;
      public string PasswordHash { get; set; } = string.Empty;
   }
}
