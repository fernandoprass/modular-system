using Shared.Domain.Entities;

namespace Shared.Domain.Interfaces;

internal interface IParameterOverrideRepository
{
   Task<ParameterOverride?> GetByIdAsync(Guid id);
   Task AddAsync(ParameterOverride parameterOverride);
   void Update(ParameterOverride parameterOverride);
   Task DeleteAsync(Guid id);
   Task<ParameterOverride?> GetByParameterIdAndOwnerIdAsync(Guid parameterId, Guid ownerId);
}
