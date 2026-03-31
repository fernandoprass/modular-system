using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IParameterValidator
   {
      Result ValidateCreate(ParameterCreateRequest request, bool keyExists);
      Result ValidateUpdate(Parameter? parameter, ParameterUpdateRequest request);
      Result ValidateCustomerUpdate(Parameter? parameter, ParameterCustomerUpdateRequest request);
   }
}
