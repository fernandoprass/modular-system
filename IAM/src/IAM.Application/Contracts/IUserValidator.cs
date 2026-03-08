using IAM.Domain.DTOs.Requests;
using IAM.Domain.Entities;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IUserValidator
   {
      Result ValidateCreate(UserCreateRequest request, bool customerExists, bool emailAlreadyExists);

      Result ValidateUpdate(Guid? id, UserUpdateRequest request);

      Result ValidateUpdatePassword(User? user, UserUpdatePasswordRequest request);
   }
}
