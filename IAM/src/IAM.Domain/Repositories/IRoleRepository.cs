using IAM.Domain.Entities;

namespace IAM.Domain.Repositories
{
   public interface IRoleRepository
   {
      Task<Role?> GetByIdAsync(Guid id);
      Task AddAsync(Role role);
      void Update(Role role);
      Task DeleteAsync(Guid id);
   }
}
