using IAM.Application.Contracts;
using IAM.Domain;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.Messages.Info;
using IAM.Domain.QueryRepositories;
using IAM.Domain.Repositories;
using Myce.Response;
using System.Globalization;

namespace IAM.Application.Services
{
   public class ParameterService(
       IUnitOfWork unitOfWork,
       IUserContext userContext,
       IParameterValidator parameterValidator,
       IParameterRepository parameterRepository,
       IParameterCustomerRepository parameterCustomerRepository,
       IParameterQueryRepository parameterQueryRepository) : BaseService(userContext), IParameterService
   {
      private readonly IUnitOfWork _unitOfWork = unitOfWork;
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
         return await _parameterQueryRepository.GetByModuleGroupAndKeyAsync(key);
      }

      public async Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request)
      {
         var existing = await _parameterRepository.GetByKeyAsync(request.Key);
         var validation = _parameterValidator.ValidateCreate(request, existing != null);
         if (validation.HasError) return Result<ParameterDto>.Failure(validation.Messages);


         var parameterKey = new ParameterKey(request.Key);

         var parameter = new Parameter
         {
            Module = parameterKey.Module,
            Group = parameterKey.Group,  
            Name = parameterKey.Name,
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Value = request.Value,
            ListItems = request.ListItems,
            ExternalListEndpoint = request.ExternalListEndpoint,
            IsCustomerEditable = request.IsCustomerEditable,
            IsVisible = request.IsVisible
         };

         await _unitOfWork.Parameters.AddAsync(parameter);
         await _unitOfWork.SaveChangesAsync();

         return Result<ParameterDto>.Success(parameter.ToParameterDto());
      }

      public async Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request)
      {
         var parameter = await _parameterRepository.GetByIdAsync(id);
         var validation = _parameterValidator.ValidateUpdate(parameter, request);
         if (validation.HasError) return Result.Failure(validation.Messages);

         parameter.Title = request.Name;
         parameter.Description = request.Description;
         parameter.Value = request.Value;
         parameter.ListItems = request.ListItems;
         parameter.ExternalListEndpoint = request.ExternalListEndpoint;
         parameter.IsCustomerEditable = request.IsCustomerEditable;
         parameter.IsVisible = request.IsVisible;

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

         var customerId = _userContext.CustomerId;
         var overrideRecord = await _parameterCustomerRepository.GetByParameterAndCustomerAsync(id, customerId);

         if (overrideRecord == null)
         {
            overrideRecord = new ParameterCustomer
            {
               ParameterId = parameter.Id,
               CustomerId = customerId,
               Value = request.Value
            };
            await _unitOfWork.ParameterCustomers.AddAsync(overrideRecord);
         }
         else
         {
            overrideRecord.Value = request.Value;
            _unitOfWork.ParameterCustomers.Update(overrideRecord);
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
         return await _parameterQueryRepository.GetValueAsync(key, _userContext.CustomerId);
      }
   }
}
