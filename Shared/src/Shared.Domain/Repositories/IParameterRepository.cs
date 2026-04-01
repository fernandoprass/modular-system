using Shared.Domain.Entities;

namespace Shared.Domain.Repositories
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
