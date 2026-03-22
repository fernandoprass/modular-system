using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.Repositories;

public interface IUserRepository
{
   Task AddAsync(User user);
   void Update(User user);
   Task DeleteAsync(Guid id);
   Task<User?> GetByIdAsync(Guid id);
   Task<IEnumerable<User>> GetByCustomerIdAsync(Guid customerId);
   Task<bool> ExistsAsync(Guid id);
}