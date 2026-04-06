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

   [HttpGet("{id:guid}")]
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
      var parameter = await _parameterService.GetValueAsync(key);
      return OkOrNotFound(parameter);
   }

   [HttpPut("{id:guid}/override")]
   [Authorize]
   public async Task<IActionResult> SaveOverride(Guid id, ParameterOwnerUpdateRequest request)
   {
      var result = await _parameterService.SaveOverrideValueAsync(id, request);
      return OkOrNotFound(result);
   }

   [HttpDelete("{id:guid}/override")]
   [Authorize]
   public async Task<IActionResult> Delete(Guid id)
   {
      var result = await _parameterService.DeleteOverrideValueAsync(id);
      return OkOrNotFound(result);
   }
}
