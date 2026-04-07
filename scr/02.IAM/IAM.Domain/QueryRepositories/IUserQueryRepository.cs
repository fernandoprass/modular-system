using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.QueryRepositories;

public interface IUserQueryRepository
{
   Task<UserDto?> GetByIdAsync(Guid id);
   Task<Guid> GetIdByEmailAsync(string email);
   Task<IEnumerable<UserLiteDto>> GetByCustomerIdAsync(Guid customerId);
   Task<User?> GetByEmailAsync(string email);
   Task<UserPasswordDto?> GetByEmailWithPasswordAsync(string email);
}