using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Contracts;
using Shared.Domain.DTOs.Requests;

namespace IAM.API.Controllers;

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
   public async Task<IActionResult> Get(ParameterSearchRequest request)
   {
      var parameters = await _parameterService.GetAsync(request);
      return OkOrNotFound(parameters);
   }

   [HttpGet("key/{key}")]
   [Authorize]
   public async Task<IActionResult> GetByKey(string key)
   {
      var parameter = await _parameterService.GetByKeyAsync(key);
      return OkOrNotFound(parameter);
   }

   [HttpDelete("{id}")]
   [Authorize]
   //[Authorize(Roles = "Admin")]
   public async Task<IActionResult> Delete(Guid id)
   {
      var result = await _parameterService.DeleteAsync(id);
      return OkOrNotFound(result);
   }

   [HttpPut("{id}/value")]
   [Authorize]
   public async Task<IActionResult> SaveOverride(Guid id, ParameterOwnerUpdateRequest request)
   {
      var result = await _parameterService.SaveOwnerValueAsync(id, request);
      return OkOrNotFound(result);
   }

   [HttpDelete("{parameterId:guid}/customer/{customerId:guid}")]
   [Authorize]
   public async Task<IActionResult> Delete(Guid parameterId, Guid customerId)
   {
      var result = await _parameterService.DeleteOwnerValueAsync(parameterId, customerId);
      return OkOrNotFound(result);
   }
}
