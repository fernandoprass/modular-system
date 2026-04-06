using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/iam/roles")]
[Authorize]
public class RoleController(IRoleService roleService) : BaseController
{
   private readonly IRoleService _roleService = roleService;

   [HttpGet]
   public async Task<IActionResult> GetAll()
   {
      var result = await _roleService.GetAllAsync();
      return OkOrNotFound(result);
   }

   [HttpPost]
   public async Task<IActionResult> Create([FromBody] RoleCreateRequest request)
   {
      var result = await _roleService.CreateAsync(request);
      return OkOrNotFound(result);
   }

   [HttpPut("{id:guid}")]
   public async Task<IActionResult> Update(Guid id, [FromBody] RoleUpdateRequest request)
   {
      var result = await _roleService.UpdateAsync(id, request);
      return OkOrNotFound(result);
   }

   [HttpPost("assign")]
   public async Task<IActionResult> AssignToUser([FromBody] RoleAssignRequest request)
   {
      var result = await _roleService.AssignToUserAsync(request);
      return OkOrNotFound(result);
   }

   [HttpGet("user/{userId:guid}/features")]
   public async Task<IActionResult> GetUserFeatures(Guid userId)
   {
      var result = await _roleService.GetUserFeaturesAsync(userId);
      return OkOrNotFound(result);
   }
}
