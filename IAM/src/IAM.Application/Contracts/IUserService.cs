using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface IUserService
{
   Task<UserDto?> GetByIdAsync(Guid id);
   Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId);
   Task<Result<UserDto>> CreateUserAsync(UserCreateRequest request);
   Task<Result> UpdateAsync(Guid id, UserUpdateRequest request);
   Task<Result> UpdatePasswordAsync(UserUpdatePasswordRequest request);
   Task<Result> DeleteAsync(Guid id);
   Task UpdateLastLoginAsync(Guid id);
}