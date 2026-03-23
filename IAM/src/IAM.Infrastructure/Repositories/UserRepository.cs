using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IamDbContext context) : base(context)
    {
    }

   public async Task<IEnumerable<User>> GetByCustomerIdAsync(Guid customerId)
   {
      return await _context.Users
                    .Where(u => u.CustomerId == customerId)
                    .ToListAsync();
   }

   public void Update(User user)
    {
        _dbSet.Update(user);
    }
}