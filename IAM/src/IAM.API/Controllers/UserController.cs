using IAM.Application.Contracts;
using IAM.Application.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myce.Response;
using Myce.Response.Messages;

namespace IAM.API.Controllers;

[ApiVersion("1.0")]
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
      // Returns a Result with the collection
      return Ok(Result<IEnumerable<UserLiteDto>>.Success(users));
   }

   [HttpPost("")]
   public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
   {
      if (!ModelState.IsValid)
      {
         // Ideally, convert ModelState errors to your Result.Messages
         return BadRequest(Result.Failure(new ErrorMessage("INVALID_INPUT", "Please check the provided data.")));
      }

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
   public async Task<IActionResult> Login([FromBody] LoginRequest request)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest(Result.Failure(new ErrorMessage("INVALID_LOGIN_INPUT", "Email and password are required.")));
      }

      var response = await _authService.LoginAsync(request);
      if (response == null)
      {
         var error = Result.Failure(new ErrorMessage("UNAUTHORIZED", "Invalid email or password"));
         return Unauthorized(error);
      }

      return Ok(Result<LoginResponse>.Success(response));
   }
}