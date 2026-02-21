using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.Mappers
{
   public static class UserToUserDto
   {
      public static UserDto ToUserDto(this User user, string customerName)
      {
         return new UserDto
         {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CustomerId = user.CustomerId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            EmailVerifiedAt = user.EmailVerifiedAt,
            LastLoginAt = user.LastLoginAt,
            CustomerName = customerName
         };
      }
   }
}
