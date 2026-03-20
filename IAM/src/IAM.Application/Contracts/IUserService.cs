using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface IUserService
{
   Task<UserDto?> GetByIdAsync(Guid id);
   Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId);
   Task<Result<UserDto>> CreateUserAsync(UserCreateRequest request,
                                         bool customerExists);
   Task<Result> UpdateAsync(Guid id, UserUpdateRequest request);
   Task<Result> UpdatePasswordAsync(UserUpdatePasswordRequest request);
   Task<Result> DeleteAsync(Guid id);
   Task<Result> UpdateLastLoginAsync(Guid id);
   Task<Result> ValidateUserForNewCustomerAsync(CustomerUserCreateRequest request);
}