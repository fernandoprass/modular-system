using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IUserFluentValidator
   {
      Result ValidateCreate(UserCreateRequest request);

      Result ValidateUpdate(Guid? id, UserUpdateRequest request);
   }
}
