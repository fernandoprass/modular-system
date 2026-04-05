using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.QueryRepositories;
using Myce.Response;
using Shared.Application.Services;
using Shared.Domain.Interfaces;
using Shared.Domain.Messages;

namespace IAM.Application.Services;

public class RoleService(
   IIamUnitOfWork iamUnitOfWork,
   IUserContext userContext,
   IRoleValidator roleValidator,
   IRoleQueryRepository roleQueryRepository,
   IUserQueryRepository userQueryRepository) : BaseService(userContext), IRoleService
{
   private readonly IIamUnitOfWork _iamUnitOfWork = iamUnitOfWork;
   private readonly IRoleValidator _roleValidator = roleValidator;
   private readonly IRoleQueryRepository _roleQueryRepository = roleQueryRepository;
   private readonly IUserQueryRepository _userQueryRepository = userQueryRepository;

   public async Task<Result<RoleDto>> CreateAsync(RoleCreateRequest request)
   {
      return await ExecuteIfUserOwnsAsync(request.CustomerId, async () =>
      {
         var nameExists = await _roleQueryRepository.NameExistsAsync(request.Name, request.CustomerId);
         var validation = _roleValidator.ValidateCreate(request, nameExists);

         if (!validation.IsSuccess)
            return Result<RoleDto>.Failure(validation.Messages);

         var role = Role.Create(request.Name, request.CustomerId, request.IsDefault);

         await _iamUnitOfWork.Roles.AddAsync(role);
         await _iamUnitOfWork.SaveChangesAsync();

         return Result<RoleDto>.Success(new RoleDto(role.Id, role.Name, role.CustomerId, role.IsDefault, Enumerable.Empty<FeatureDto>()));
      });
   }

   public async Task<Result> UpdateAsync(Guid id, RoleUpdateRequest request)
   {
      var role = await _iamUnitOfWork.Roles.GetByIdAsync(id);

      if (role == null)
         return Result.Failure(new NotFoundError("Role"));

      return await ExecuteIfUserOwnsAsync(role.CustomerId, async () =>
      {
         var validation = _roleValidator.ValidateUpdate(id, request, role.IsDefault);

         if (!validation.IsSuccess)
            return validation;

         role.Update(request.Name);
         _iamUnitOfWork.Roles.Update(role);
         await _iamUnitOfWork.SaveChangesAsync();

         return Result.Success();
      });
   }

   public async Task<Result> AssignToUserAsync(RoleAssignRequest request)
   {
      var user = await _iamUnitOfWork.Users.GetByIdAsync(request.UserId);
      
      if (user == null)
         return Result.Failure(new NotFoundError("User"));

      return await ExecuteIfUserOwnsAsync(user.CustomerId, async () =>
      {
         // Simple check for all roles existing and being within same customer or global
         var roles = await _roleQueryRepository.GetAllAsync(_userContext.UserOwnerId);
         var allRequestedRolesExist = request.RoleIds.All(rid => roles.Any(r => r.Id == rid));

         var validation = _roleValidator.ValidateAssign(request, true, allRequestedRolesExist);

         if (!validation.IsSuccess)
            return validation;

         user.ClearRoles();
         foreach (var roleId in request.RoleIds)
         {
            user.AddRole(roleId);
         }

         _iamUnitOfWork.Users.Update(user);
         await _iamUnitOfWork.SaveChangesAsync();

         return Result.Success();
      });
   }

   public async Task<Result<IEnumerable<RoleDto>>> GetAllAsync()
   {
      var roles = await _roleQueryRepository.GetAllAsync(_userContext.UserOwnerId);
      var dtos = roles.Select(r => new RoleDto(
         r.Id, 
         r.Name, 
         r.CustomerId, 
         r.IsDefault, 
         r.RoleFeatures.Select(rf => new FeatureDto(rf.Feature.Id, rf.Feature.Name, rf.Feature.Description, rf.Feature.Group))
      ));

      return Result<IEnumerable<RoleDto>>.Success(dtos);
   }

   public async Task<Result<IEnumerable<FeatureDto>>> GetUserFeaturesAsync(Guid userId)
   {
      var user = await _iamUnitOfWork.Users.GetByIdAsync(userId);
      if (user == null)
         return Result<IEnumerable<FeatureDto>>.Failure(new NotFoundError("User"));

      return await ExecuteIfUserOwnsAsync(user.CustomerId, async () =>
      {
         var features = await _roleQueryRepository.GetUserFeaturesAsync(userId);
         var dtos = features.Select(f => new FeatureDto(f.Id, f.Name, f.Description, f.Group));
         return Result<IEnumerable<FeatureDto>>.Success(dtos);
      });
   }
}
