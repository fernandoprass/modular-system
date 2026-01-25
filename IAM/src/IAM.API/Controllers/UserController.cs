using IAM.Core.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IAM.API.Controllers;

[Route("api/[controller]")]
public class UserController : BaseController
{
   private readonly IUserService _userService;
   private readonly IAuthService _authService;

   public UserController(IUserService userService, IAuthService authService)
   {
      _userService = userService;
      _authService = authService;
   }

   [HttpGet]
   [Authorize]
   public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
   {
      var users = await _userService.GetAllAsync();
      return Ok(users);
   }

   [HttpGet("{id}")]
   [Authorize]
   public async Task<Results<Ok<UserDto>, NotFound>> GetById(Guid id)
   {
      var user = await _userService.GetByIdAsync(id);
      return OkOrNotFound(user);
   }

   [HttpGet("by-email/{email}")]
   [Authorize]
   public async Task<Results<Ok<UserDto>, NotFound>> GetByEmail(string email)
   {
      var user = await _userService.GetByEmailAsync(email);
      return OkOrNotFound(user);
   }

   [HttpGet("by-customer/{customerId}")]
   [Authorize]
   public async Task<ActionResult<IEnumerable<UserDto>>> GetByCustomerId(Guid customerId)
   {
      var users = await _userService.GetByCustomerIdAsync(customerId);
      return Ok(users);
   }

   [HttpPost("register")]
   public async Task<ActionResult<User>> Register([FromBody] CreateUserRequest request)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest(ModelState);
      }

      try
      {
         var createdUser = await _userService.CreateUserAsync(request);
         return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
      }
      catch (InvalidOperationException ex)
      {
         return BadRequest(new { message = ex.Message });
      }
   }

   [HttpPost("login")]
   public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest(ModelState);
      }

      var response = await _authService.LoginAsync(request);
      if (response == null)
      {
         return Unauthorized(new { message = "Invalid email or password" });
      }

      return Ok(response);
   }

   [HttpPut("{id}")]
   [Authorize]
   public async Task<Results<NoContent, NotFound>> Update(Guid id, [FromBody] User user)
   {
      return await ExecuteIfExistsAsync(
        () => _userService.ExistsAsync(id),
        async () => await _userService.UpdateAsync(user));
   }

   [HttpDelete("{id}")]
   [Authorize]
   public async Task<Results<NoContent, NotFound>> Delete(Guid id)
   {
      return await ExecuteIfExistsAsync(
         () => _userService.ExistsAsync(id),
         async () => await _userService.DeleteAsync(id));
   }
}