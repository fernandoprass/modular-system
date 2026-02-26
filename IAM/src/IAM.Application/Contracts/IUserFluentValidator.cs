using IAM.Domain.DTOs;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IUserFluentValidator
   {
      Result ValidateCreate(UserCreateRequest request);
   }
}
