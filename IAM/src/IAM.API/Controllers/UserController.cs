using Asp.Versioning;
using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myce.Response;
using Myce.Response.Messages;

namespace IAM.API.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/iam/users")]
public class UserController : BaseController
{
   private readonly IUserService _userService;
   private readonly IAuthService _authService;

   public UserController(IUserService userService, IAuthService authService)
   {
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

   [HttpGet("by-email/{email}")]
   [Authorize]
   public async Task<IActionResult> GetByEmail(string email)
   {
      var user = await _userService.GetByEmailAsync(email);
      return OkOrNotFound(user);
   }

   [HttpGet("by-customer/{customerId}")]
   [Authorize]
   public async Task<IActionResult> GetByCustomerId(Guid customerId)
   {
      var users = await _userService.GetByCustomerIdAsync(customerId);

      return Ok(Result<IEnumerable<UserLiteDto>>.Success(users));
   }

   [HttpPost("")]
   public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
   {
      var user = await _userService.CreateUserAsync(request);

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
      return await ExecuteIfExistsAsync(
          () => _userService.ExistsAsync(id),
          async (exists) => await _userService.DeleteAsync(id));
   }

   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
   {
      var response = await _authService.LoginAsync(request);

      return response .IsValid ? OkOrNotFound(response) : Unauthorized(response);

   }
}