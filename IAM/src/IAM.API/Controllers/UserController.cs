using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Application.Extensions;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/iam/users")]
public class UserController : BaseController
{
   private readonly IRegisterOrchestrator _registerOrchestrator;
   private readonly IUserService _userService;
   private readonly IAuthService _authService;

   public UserController(
      IRegisterOrchestrator registerOrchestrator,
      IUserService userService, 
      IAuthService authService)
   {
      _registerOrchestrator = registerOrchestrator;
      _userService = userService;
      _authService = authService;
   }

   [HttpGet("{id}")]
   [Authorize]
   public async Task<IActionResult> GetById(Guid id)
   {
      var user = await _userService.GetByIdAsync(id);
      return OkOrNotFound(user);
   }

   [HttpGet("by-customer/{customerId}")]
   [Authorize]
   public async Task<IActionResult> GetByCustomerId(Guid customerId)
   {
      var users = await _userService.GetByCustomerIdAsync(customerId);

      return OkOrNotFound(users);
   }

   [HttpPost("")]
   public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
   {
      var operatorCustomerId = User.GetCustomerId();
      var user = await _registerOrchestrator.RegisterUserAsync(request, operatorCustomerId);

      return OkOrNotFound(user);
   }

   [HttpPut("{id}")]
   [Authorize]
   public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest user)
   {
      var response = await _userService.UpdateAsync(id, user);

      return OkOrNotFound(response);
   }

   [HttpDelete("{id}")]
   [Authorize]
   public async Task<IActionResult> Delete(Guid id)
   {
      var response = await _userService.DeleteAsync(id);

      return OkOrNotFound(response);
   }

   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
   {
      var response = await _authService.LoginAsync(request);

      return response .IsValid ? OkOrNotFound(response) : Unauthorized(response);

   }
}