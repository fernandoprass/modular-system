using Myce.Response;
using Shared.Application.Contracts;
using Shared.Domain;
using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Interfaces;
using Shared.Domain.Mappers;
using Shared.Domain.Messages;
using Shared.Domain.QueryRepositories;
using Shared.Domain.Repositories;
using System.Globalization;

namespace Shared.Application.Services;

public class ParameterService(
    ISharedUnitOfWork unitOfWork,
    IUserContext userContext,
    IParameterValidator parameterValidator,
    IParameterRepository parameterRepository,
    IParameterOverrideRepository parameterOverrideRepository,
    IParameterQueryRepository parameterQueryRepository) : BaseService(userContext), IParameterService
{
   private readonly ISharedUnitOfWork _unitOfWork = unitOfWork;
   private readonly IParameterValidator _parameterValidator = parameterValidator;
   private readonly IParameterRepository _parameterRepository = parameterRepository;
   private readonly IParameterOverrideRepository _parameterOverrideRepository = parameterOverrideRepository;
   private readonly IParameterQueryRepository _parameterQueryRepository = parameterQueryRepository;

   public async Task<ParameterDto?> GetByIdAsync(Guid id)
   {
      return await _parameterQueryRepository.GetByIdAsync(id);
   }

   public async Task<IEnumerable<ParameterLiteDto>> GetAsync(ParameterSearchRequest request)
   {
      return await _parameterQueryRepository.GetAllAsync(request);
   }

   public async Task<ParameterDto?> GetByKeyAsync(string key)
   {
      var parameterKey = new ParameterKey(key);
      return await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(parameterKey.Module, parameterKey.Group, parameterKey.Name);
   }

   public async Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request)
   {
      var keyExists = await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(request.Module, request.Group, request.Name);
      var validation = _parameterValidator.ValidateCreate(request, keyExists != null);
      if (validation.HasError) return Result<ParameterDto>.Failure(validation.Messages);

      var parameter = Parameter.Create(
          request.Module,
          request.Group,
          request.Name,
          request.Title,
          request.Description,
          request.Type,
          request.Value,
          request.ValidationRegex,
          request.ValidationErrorCustomMessage,
          request.ListItems,
          request.ExternalListEndpoint,
          request.OverrideType,
          request.IsVisible);

      await _unitOfWork.Parameters.AddAsync(parameter);
      await _unitOfWork.SaveChangesAsync();

      return Result<ParameterDto>.Success(parameter.ToParameterDto());
   }

   public async Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request)
   {
      var parameter = await _parameterRepository.GetByIdAsync(id);
      var keyExists = await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(request.Module, request.Group, request.Name);
      var validation = _parameterValidator.ValidateUpdate(parameter != null, keyExists != null, request);
      if (validation.HasError) return Result.Failure(validation.Messages);

      parameter.Update(
          request.Module,
          request.Group,
          request.Name,
          request.Title,
          request.Description,
          request.Type,
          request.Value,
          request.ValidationRegex,
          request.ValidationErrorCustomMessage,
          request.ListItems,
          request.ExternalListEndpoint,
          request.OverrideType,
          request.IsVisible);

      _unitOfWork.Parameters.Update(parameter);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> DeleteAsync(Guid id)
   {
      var parameter = await _parameterRepository.GetByIdAsync(id);
      if (parameter == null) return Result.Failure(new NotFoundError(Const.Entity.Parameter));

      await _unitOfWork.Parameters.DeleteAsync(id);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> SaveOwnerValueAsync(Guid parameterId, ParameterOwnerUpdateRequest request)
   {
      var parameter = await _parameterRepository.GetByIdAsync(parameterId);
      var validation = _parameterValidator.ValidateOwnerUpdate(parameter, request);
      if (validation.HasError) return Result.Failure(validation.Messages);

      var ownerId = GetOwnerId(parameter.OverrideType);

      var parameterOverride = await _parameterOverrideRepository.GetByParameterIdAndOwnerIdAsync(parameterId, ownerId);

      if (parameterOverride == null)
      {
         parameterOverride = ParameterOverride.Create(parameterId, ownerId, request.Value);

         await _unitOfWork.ParameterOverrides.AddAsync(parameterOverride);
      }
      else
      {
         parameterOverride.Update(request.Value);
         _unitOfWork.ParameterOverrides.Update(parameterOverride);
      }

      await _unitOfWork.SaveChangesAsync();
      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> DeleteOwnerValueAsync(Guid parameterId, Guid ownerId)
   {
      var parameterOverride = await _parameterOverrideRepository.GetByParameterIdAndOwnerIdAsync(parameterId, ownerId);

      if (parameterOverride == null) return Result.Failure(new NotFoundError(Const.Entity.ParameterOverride));

      if (parameterOverride != null)
      {
         await _unitOfWork.ParameterOverrides.DeleteAsync(parameterOverride.Id);
         await _unitOfWork.SaveChangesAsync();
      }

      return Result.Success(new SuccessInfo());
   }

   public async Task<bool> GetBoolAsync(string key)
   {
      var value = await GetResolvedValueAsync(key);
      return bool.TryParse(value, out var result) && result;
   }

   public async Task<int> GetIntAsync(string key)
   {
      var value = await GetResolvedValueAsync(key);
      return int.TryParse(value, out var result) ? result : 0;
   }

   public async Task<decimal> GetDecimalAsync(string key)
   {
      var value = await GetResolvedValueAsync(key);
      return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0m;
   }

   public async Task<DateTime> GetDateTimeAsync(string key)
   {
      var value = await GetResolvedValueAsync(key);
      return DateTime.TryParse(value, out var result) ? result : DateTime.MinValue;
   }

   public async Task<string> GetStringAsync(string key)
   {
      return await GetResolvedValueAsync(key) ?? string.Empty;
   }

   private async Task<string?> GetResolvedValueAsync(string key)
   {
      return await _parameterQueryRepository.GetValueAsync(key, _userContext.UserOwnerId);
   }

   private Guid GetOwnerId(ParameterOverrideType overrideType) 
   {
      return overrideType switch
      {
         ParameterOverrideType.UserOwnerId => _userContext.UserOwnerId,
         ParameterOverrideType.UserId => _userContext.UserId,
         _ => throw new InvalidOperationException("Invalid override type")
      };
   }
}
