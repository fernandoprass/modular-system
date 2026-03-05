using IAM.Domain.DTOs.Requests;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IUserFluentValidator
   {
      Task<Result> ValidateCreateAsync(UserCreateRequest request);

      Task<Result> ValidateUpdateAsync(Guid? id, UserUpdateRequest request);
   }
}
