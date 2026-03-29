using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers
{
   [ApiVersion(1)]
   [Route("api/v{version:apiVersion}/iam/parameters")]
   public class ParameterController(IParameterService parameterService) : BaseController
   {
      private readonly IParameterService _parameterService = parameterService;

      [HttpGet("{id}")]
      [Authorize]
      public async Task<IActionResult> GetById(Guid id)
      {
         var parameter = await _parameterService.GetByIdAsync(id);
         return OkOrNotFound(parameter);
      }

      [HttpGet]
      [Authorize]
      public async Task<IActionResult> GetAll()
      {
         var parameters = await _parameterService.GetAllAsync();
         return OkOrNotFound(parameters);
      }

      [HttpGet("group/{group}")]
      [Authorize]
      public async Task<IActionResult> GetByGroup(string group)
      {
         var parameters = await _parameterService.GetByGroupAsync(group);
         return OkOrNotFound(parameters);
      }

      [HttpGet("group/{group}/key/{key}")]
      [Authorize]
      public async Task<IActionResult> GetByGroupAndKey(string group, string key)
      {
         var parameter = await _parameterService.GetByGroupAndKeyAsync(group, key);
         return OkOrNotFound(parameter);
      }

      [HttpPost]
      [Authorize]
      //[Authorize(Roles = "Admin")]
      public async Task<IActionResult> Create(ParameterCreateRequest request)
      {
         var result = await _parameterService.CreateAsync(request);
         return OkOrNotFound(result);
      }

      [HttpPut("{id}")]
      [Authorize]
      //[Authorize(Roles = "Admin")]
      public async Task<IActionResult> Update(Guid id, ParameterUpdateRequest request)
      {
         var result = await _parameterService.UpdateAsync(id, request);
         return OkOrNotFound(result);
      }

      [HttpDelete("{id}")]
      [Authorize]
      //[Authorize(Roles = "Admin")]
      public async Task<IActionResult> Delete(Guid id)
      {
         var result = await _parameterService.DeleteAsync(id);
         return OkOrNotFound(result);
      }

      [HttpPut("group/{group}/key/{key}/override")]
      [Authorize]
      public async Task<IActionResult> SaveOverride(string group, string key, ParameterCustomerUpdateRequest request)
      {
         var result = await _parameterService.SaveCustomerOverrideAsync(group, key, request);
         return OkOrNotFound(result);
      }

      [HttpDelete("group/{group}/key/{key}/override")]
      [Authorize]
      public async Task<IActionResult> DeleteOverride(string group, string key)
      {
         var result = await _parameterService.DeleteCustomerOverrideAsync(group, key);
         return OkOrNotFound(result);
      }
   }
}
