using IAM.Domain.DTOs.Requests;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface ICustomerValidator
{
   Result ValidateCreate(CustomerCreateRequest request);
   Result ValidateUpdate(Guid id, CustomerUpdateRequest request);
}
