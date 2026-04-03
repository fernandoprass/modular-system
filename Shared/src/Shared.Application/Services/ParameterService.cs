using Myce.Response;
using Shared.Application.Contracts;
using Shared.Domain;
using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Shared.Domain.Entities;
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
    IParameterCustomerRepository parameterCustomerRepository,
    IParameterQueryRepository parameterQueryRepository) : BaseService(userContext), IParameterService
{
   private readonly ISharedUnitOfWork _unitOfWork = unitOfWork;
   private readonly IParameterValidator _parameterValidator = parameterValidator;
   private readonly IParameterRepository _parameterRepository = parameterRepository;
   private readonly IParameterCustomerRepository _parameterCustomerRepository = parameterCustomerRepository;
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
      var existing = await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(request.Module, request.Group, request.Name);
      var validation = _parameterValidator.ValidateCreate(request, existing != null);
      if (validation.HasError) return Result<ParameterDto>.Failure(validation.Messages);

      var parameter = Parameter.Create(
          request.Module,
          request.Group,
          request.Name,
          request.Title,
          request.Description,
          request.Type,
          request.Value,
          request.ListItems,
          request.ExternalListEndpoint,
          request.IsCustomerEditable,
          request.IsVisible);

      await _unitOfWork.Parameters.AddAsync(parameter);
      await _unitOfWork.SaveChangesAsync();

      return Result<ParameterDto>.Success(parameter.ToParameterDto());
   }

   public async Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request)
   {
      var parameter = await _parameterRepository.GetByIdAsync(id);
      var validation = _parameterValidator.ValidateUpdate(parameter, request);
      if (validation.HasError) return Result.Failure(validation.Messages);

      parameter.Update(
          request.Module,
          request.Group,
          request.Name,
          request.Title,
          request.Description,
          request.Type,
          request.Value,
          request.ListItems,
          request.ExternalListEndpoint,
          request.IsCustomerEditable,
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

   public async Task<Result> SaveCustomerAsync(Guid id, ParameterCustomerUpdateRequest request)
   {
      var parameter = await _parameterRepository.GetByIdAsync(id);
      var validation = _parameterValidator.ValidateCustomerUpdate(parameter, request);
      if (validation.HasError) return Result.Failure(validation.Messages);

      var customerId = _userContext.OwnerId;

      var parameterCustomer = await _parameterCustomerRepository.GetByParameterAndCustomerAsync(id, customerId);

      if (parameterCustomer == null)
      {
         parameterCustomer = ParameterCustomer.Create(id, customerId, request.Value);

         await _unitOfWork.ParameterCustomers.AddAsync(parameterCustomer);
      }
      else
      {
         parameterCustomer.Update(request.Value);
         _unitOfWork.ParameterCustomers.Update(parameterCustomer);
      }

      await _unitOfWork.SaveChangesAsync();
      return Result.Success(new SuccessInfo());
   }

   public async Task<Result> DeleteCustomerValueAsync(Guid parameterId, Guid customerId)
   {
      var parameterCustomer = await _parameterCustomerRepository.GetByParameterAndCustomerAsync(parameterId, customerId);

      if (parameterCustomer == null) return Result.Failure(new NotFoundError(Const.Entity.ParameterCustomer));

      if (parameterCustomer != null)
      {
         await _unitOfWork.ParameterCustomers.DeleteAsync(parameterCustomer.Id);
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
      return await _parameterQueryRepository.GetValueAsync(key, _userContext.OwnerId);
   }
}
