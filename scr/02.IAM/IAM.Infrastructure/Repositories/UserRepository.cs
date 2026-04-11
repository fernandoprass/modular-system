using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace IAM.Infrastructure.Repositories;

public class UserRepository(IamDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
{
   public async Task<IEnumerable<User>> GetByCustomerIdAsync(Guid customerId)
   {
      return await dbContext.Users
                    .Where(u => u.CustomerId == customerId)
                    .ToListAsync();
   }
}