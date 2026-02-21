using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface IUserService
{
   Task<UserDto?> GetByIdAsync(Guid id);
   Task<UserDto?> GetByEmailAsync(string email);
   Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId);
   Task<Result<UserDto>> CreateUserAsync(UserCreateRequest request);
   Task<UserDto?> ValidateCredentialsAsync(string email, string password);
   Task<Result> UpdateAsync(Guid id, UserUpdateRequest user);
   Task UpdatePasswordAsync(UserUpdatePasswordRequest request);
   Task DeleteAsync(Guid id);
   Task<bool> ExistsAsync(Guid id);
   Task UpdateLastLoginAsync(Guid id);
}