using IAM.Application.Contracts;
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

      public async Task<IEnumerable<ParameterLiteDto>> GetAllAsync()
      {
         return await _parameterQueryRepository.GetAllAsync();
      }

      public async Task<IEnumerable<ParameterLiteDto>> GetByGroupAsync(string group)
      {
         return await _parameterQueryRepository.GetByGroupAsync(group);
      }

      public async Task<ParameterDto?> GetByGroupAndKeyAsync(string group, string key)
      {
         return await _parameterQueryRepository.GetByGroupAndKeyAsync(group, key);
      }

      public async Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request)
      {
         var existing = await _parameterRepository.GetByGroupAndKeyAsync(request.Group, request.Key);
         var validation = _parameterValidator.ValidateCreate(request, existing != null);
         if (validation.HasError) return Result<ParameterDto>.Failure(validation.Messages);

         var parameter = new Parameter
         {
            Group = request.Group,
            Key = request.Key,
            Name = request.Name,
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

         parameter.Name = request.Name;
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
         if (parameter == null) return Result.Failure(new NotFoundError("Parameter"));

         await _unitOfWork.Parameters.DeleteAsync(id);
         await _unitOfWork.SaveChangesAsync();

         return Result.Success(new SuccessInfo());
      }

      public async Task<Result> SaveCustomerOverrideAsync(string group, string key, ParameterCustomerUpdateRequest request)
      {
         var parameter = await _parameterRepository.GetByGroupAndKeyAsync(group, key);
         var validation = _parameterValidator.ValidateCustomerUpdate(parameter, request);
         if (validation.HasError) return Result.Failure(validation.Messages);

         var customerId = _userContext.CustomerId;
         var overrideRecord = await _parameterCustomerRepository.GetByParameterAndCustomerAsync(parameter.Id, customerId);

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

      public async Task<Result> DeleteCustomerOverrideAsync(string group, string key)
      {
         var parameter = await _parameterRepository.GetByGroupAndKeyAsync(group, key);
         if (parameter == null) return Result.Failure(new NotFoundError("Parameter"));

         var customerId = _userContext.CustomerId;
         var overrideRecord = await _parameterCustomerRepository.GetByParameterAndCustomerAsync(parameter.Id, customerId);

         if (overrideRecord != null)
         {
            await _unitOfWork.ParameterCustomers.DeleteAsync(overrideRecord.Id);
            await _unitOfWork.SaveChangesAsync();
         }

         return Result.Success(new SuccessInfo());
      }

      public async Task<bool> GetBoolAsync(string group, string key)
      {
         var value = await GetResolvedValueAsync(group, key);
         return bool.TryParse(value, out var result) && result;
      }

      public async Task<int> GetIntAsync(string group, string key)
      {
         var value = await GetResolvedValueAsync(group, key);
         return int.TryParse(value, out var result) ? result : 0;
      }

      public async Task<decimal> GetDecimalAsync(string group, string key)
      {
         var value = await GetResolvedValueAsync(group, key);
         return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0m;
      }

      public async Task<DateTime> GetDateTimeAsync(string group, string key)
      {
         var value = await GetResolvedValueAsync(group, key);
         return DateTime.TryParse(value, out var result) ? result : DateTime.MinValue;
      }

      public async Task<string> GetStringAsync(string group, string key)
      {
         return await GetResolvedValueAsync(group, key) ?? string.Empty;
      }

      private async Task<string?> GetResolvedValueAsync(string group, string key)
      {
         return await _parameterQueryRepository.GetValueAsync(group, key, _userContext.CustomerId);
      }
   }
}
