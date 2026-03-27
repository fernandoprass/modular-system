namespace IAM.Domain.DTOs.Responses;

public sealed record UserDto
{
   public Guid Id { get; init; }
   public string Name { get; init; } = string.Empty;
   public string Email { get; init; } = string.Empty;
   public Guid CustomerId { get; init; }
   public string CustomerName { get; set; } = string.Empty;
   public bool IsActive { get; init; }
   public bool IsSuperUser { get; init; }
   public DateTime CreatedAt { get; init; }
   public DateTime? UpdatedAt { get; init; }
   public DateTime? EmailVerifiedAt { get; set; }
   public DateTime? LastLoginAt { get; set; }

   public void UpdateCustomerName(string customerName)
   {
      CustomerName = customerName;
   }
}