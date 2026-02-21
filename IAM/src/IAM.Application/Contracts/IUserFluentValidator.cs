using IAM.Domain.DTOs.Requests;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IUserFluentValidator
   {
      Result ValidateCreate(UserCreateRequest request);
   }
}
