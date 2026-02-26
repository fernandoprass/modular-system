using IAM.Application.Contracts;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using IAM.Domain.Mappers;
using IAM.Domain.Messages.Errors;
using IAM.Domain.QueryRepositories;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Myce.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IAM.Application.Services;

public interface IAuthService
{
   Task<Result<LoginResponse?>> LoginAsync(UserLoginRequest request);
}

public class AuthService : IAuthService
{
   private readonly IUserQueryRepository _userQueryRepository;
   private readonly IUserService _userService;
   private readonly string _jwtSecret;
   private readonly int _jwtExpirationHours;

   public AuthService(IUserQueryRepository userQueryRepository,
      IUserService userService,
      IConfiguration configuration)
   {
      _userQueryRepository = userQueryRepository;
      _userService = userService;
      _jwtSecret = configuration["Jwt:Secret"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure";
      _jwtExpirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "24");
   }

   public async Task<Result<LoginResponse?>> LoginAsync(UserLoginRequest request)
   {
      var user = await _userQueryRepository.GetByEmailWithPasswordAsync(request.Email);

      var isValid = user is null ? false : Argon2.Verify(user?.PasswordHash, request.Password);

      if (!isValid)
      {
         return Result<LoginResponse?>.Failure(new UnauthorizedError());
      }

      var userDto = user.ToUserDto();

      var token = GenerateJwtToken(userDto);

      //todo: should it be merged with ValidateCredentialsAsync?
      await _userService.UpdateLastLoginAsync(user.Id);

      var response = new LoginResponse
      {
         Token = token,
         ExpiresAt = DateTime.UtcNow.AddHours(_jwtExpirationHours),
         User = userDto
      };

      return Result<LoginResponse?>.Success(response);
   }

   public string GenerateJwtToken(UserDto user)
   {
      var claims = new[]
      {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim("customerId", user.CustomerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: "IAM.API",
          audience: "IAM.Client",
          claims: claims,
          expires: DateTime.UtcNow.AddHours(_jwtExpirationHours),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
   }
}