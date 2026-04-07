using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts;

public interface IRoleService
{
   Task<Result<RoleDto>> CreateAsync(RoleCreateRequest request);
   Task<Result> UpdateAsync(Guid id, RoleUpdateRequest request);
   Task<Result> AssignToUserAsync(RoleAssignRequest request);
   Task<Result<IEnumerable<RoleDto>>> GetAllAsync();
   Task<Result<IEnumerable<FeatureDto>>> GetUserFeaturesAsync(Guid userId);
}
