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

namespace Shared.Application.Services;

internal class ParameterService(
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

   #region Controller Methods
   public async Task<Result<ParameterDto>> GetByIdAsync(Guid id)
   {
      var parameter = await _parameterQueryRepository.GetByIdAsync(id);

      if (parameter == null) return Result<ParameterDto>.Failure(new NotFoundError(SharedConst.Entity.Parameter));

      return Result<ParameterDto>.Success(parameter);
   }

   public async Task<Result<IEnumerable<ParameterLiteDto>>> GetAsync(ParameterSearchRequest request)
   {
      var requestInternal = request.ToInternal(_userContext.UserOwnerId, _userContext.UserId, _userContext.IsSystemAdmin);

      var parameters = await _parameterQueryRepository.GetAllAsync(requestInternal);

      return Result<IEnumerable<ParameterLiteDto>>.Success(parameters);
   }

   public async Task<Result<ParameterValueDto>> GetValueAsync(string key)
   {
      var parameter = await _parameterQueryRepository.GetValueAsync(key, _userContext.UserOwnerId, _userContext.UserId);

      if (parameter == null) return Result<ParameterValueDto>.Failure(new NotFoundError(key));

      return Result<ParameterValueDto>.Success(parameter);
   }

   public async Task<Result> SaveOverrideValueAsync(Guid parameterId, ParameterOwnerUpdateRequest request)
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

   public async Task<Result> DeleteOverrideValueAsync(Guid parameterId)
   {
      var parameter = await _parameterRepository.GetByIdAsync(parameterId);

      if (parameter == null) return Result<ParameterValueDto>.Failure(new NotFoundError(SharedConst.Entity.Parameter));

      var ownerId = GetOwnerId(parameter.OverrideType);
      var parameterOverride = await _parameterOverrideRepository.GetByParameterIdAndOwnerIdAsync(parameterId, ownerId);

      if (parameterOverride == null) return Result.Failure(new NotFoundError(SharedConst.Entity.ParameterOverride));

      await _unitOfWork.ParameterOverrides.DeleteAsync(parameterOverride.Id);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }
   #endregion

   #region Internal Methods for Parameter Management
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

   public async Task<ParameterDto?> GetByKeyAsync(string key)
   {
      var parameterKey = new ParameterKey(key);
      return await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(parameterKey.Module, parameterKey.Group, parameterKey.Name);
   }

   public async Task<Result> DeleteAsync(Guid id)
   {
      var parameter = await _parameterRepository.GetByIdAsync(id);

      if(parameter == null) return Result.Failure(new NotFoundError(SharedConst.Entity.Parameter));

      await _unitOfWork.Parameters.DeleteAsync(id);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success(new SuccessInfo());
   }

   public Task<bool> GetBoolAsync(string key) => GetAndParseAsync<bool>(key, bool.TryParse);

   public Task<int> GetIntAsync(string key) => GetAndParseAsync<int>(key, int.TryParse);

   public Task<decimal> GetDecimalAsync(string key) => GetAndParseAsync<decimal>(key, decimal.TryParse);

   public Task<DateTime> GetDateTimeAsync(string key) => GetAndParseAsync<DateTime>(key, DateTime.TryParse);

   public async Task<string> GetStringAsync(string key)
   {
      return await GetResolvedValueAsync(key) ?? string.Empty;
   }

   private delegate bool TryParseDelegate<T>(string s, out T result);

   private async Task<T> GetAndParseAsync<T>(string key, TryParseDelegate<T> parser)
   {
      var value = await GetResolvedValueAsync(key);

      if (value == null || !parser(value, out var result))
      {
         throw new InvalidDataException($"Value '{value ?? "null"}' is invalid for parameter '{key}' and expected type {typeof(T).Name}");
      }

      return result;
   }

   private async Task<string?> GetResolvedValueAsync(string key)
   {
      var parameter = await _parameterQueryRepository.GetValueAsync(key, _userContext.UserOwnerId, _userContext.UserId);

      if (parameter == null)
      {
         throw new ArgumentNullException(nameof(key));
      }

      return parameter.Value;
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
   #endregion
}