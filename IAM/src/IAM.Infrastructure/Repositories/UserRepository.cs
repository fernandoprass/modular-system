using IAM.Domain.Entities;
using IAM.Domain.Repositories;

namespace IAM.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IamDbContext context) : base(context)
    {
    }

    public void Update(User user)
    {
        _dbSet.Update(user);
    }
}