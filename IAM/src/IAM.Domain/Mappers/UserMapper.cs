using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.Mappers
{
   public static class UserMapper
   {
      public static UserDto ToUserDto(this User user)
      {
         return new UserDto
         {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            IsSuperUser = user.IsSuperUser,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            EmailVerifiedAt = user.EmailVerifiedAt,
            LastLoginAt = user.LastLoginAt,
            CustomerId = user.CustomerId,
            CustomerName = user.Customer?.Name ?? string.Empty
         };
      }

      public static UserDto ToUserDto(this UserPasswordDto user)
      {
         return new UserDto
         {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            IsSuperUser = user.IsSuperUser,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            EmailVerifiedAt = user.EmailVerifiedAt,
            LastLoginAt = user.LastLoginAt,
            CustomerId = user.CustomerId,
            CustomerName = user.CustomerName
         };
      }

      public static UserPasswordDto ToUserPasswordDto(this User user)
      {
         return new UserPasswordDto
         {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            IsActive = user.IsActive,
            IsSuperUser = user.IsSuperUser,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            EmailVerifiedAt = user.EmailVerifiedAt,
            LastLoginAt = user.LastLoginAt,
            CustomerId = user.CustomerId,
            CustomerName = user.Customer?.Name ?? string.Empty
         };
      }
   }
}
