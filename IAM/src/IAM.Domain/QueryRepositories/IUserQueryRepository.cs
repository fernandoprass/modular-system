using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;

namespace IAM.Domain.QueryRepositories;

public interface IUserQueryRepository
{
   Task<UserDto?> GetByEmailAsync(string email);
   Task<IEnumerable<UserDto>> GetByCustomerIdAsync(Guid customerId);
   Task<IEnumerable<UserDto>> GetAllAsync();
   Task<UserPasswordDto?> GetByEmailWithPasswordAsync(string email);
}