using Shared.Domain.DTOs.Requests;
using Shared.Domain.Entities;
using Myce.Response;

namespace Shared.Application.Contracts
{
   public interface IParameterValidator
   {
      Result ValidateCreate(ParameterCreateRequest request, bool keyExists);
      Result ValidateUpdate(Parameter? parameter, ParameterUpdateRequest request);
      Result ValidateCustomerUpdate(Parameter? parameter, ParameterCustomerUpdateRequest request);
   }
}
