using IAM.Domain.Entities;

namespace IAM.Domain.Repositories
{
   public interface IParameterCustomerRepository
   {
      Task<ParameterCustomer?> GetByIdAsync(Guid id);
      Task AddAsync(ParameterCustomer parameterCustomer);
      void Update(ParameterCustomer parameterCustomer);
      Task DeleteAsync(Guid id);
      Task<ParameterCustomer?> GetByParameterAndCustomerAsync(Guid parameterId, Guid customerId);
   }
}
