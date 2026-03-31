using IAM.Domain.Entities;

namespace IAM.Domain.Repositories
{
   public interface IParameterRepository
   {
      Task<Parameter?> GetByIdAsync(Guid id);
      Task AddAsync(Parameter parameter);
      void Update(Parameter parameter);
      Task DeleteAsync(Guid id);
      Task<Parameter?> GetByKeyAsync(string key);
    }
}
