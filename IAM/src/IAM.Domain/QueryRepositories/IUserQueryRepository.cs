using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;

namespace IAM.Domain.QueryRepositories;

public interface IUserQueryRepository
{
   Task<UserDto?> GetByIdAsync(Guid id);
   Task<UserDto?> GetByEmailAsync(string email);
   Task<Guid?> GetIdByEmailAsync(string email);
   Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId);
   Task<IEnumerable<UserDto>> GetAllAsync();
   Task<UserPasswordDto?> GetByEmailWithPasswordAsync(string email);
}