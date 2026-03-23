using IAM.Domain.Entities;

namespace IAM.Domain.QueryRepositories
{
   public interface IRoleQueryRepository
   {
      Task<Role?> GetByIdAsync(Guid id);
      Task<IEnumerable<Role>> GetAllAsync(Guid customerId);
      Task<bool> NameExistsAsync(string name, Guid? customerId);
      Task<IEnumerable<Feature>> GetUserFeaturesAsync(Guid userId);
   }
}
