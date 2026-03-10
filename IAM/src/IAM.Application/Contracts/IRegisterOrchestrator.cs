using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface IRegisterOrchestrator
{
   Task<Result<UserDto>> RegisterUserAsync(UserCreateRequest request, Guid operatorCustomerId);
}
