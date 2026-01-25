using IAM.Core.Services;
using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IAM.Core.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    string GenerateJwtToken(UserDto user);
}

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly string _jwtSecret;
    private readonly int _jwtExpirationHours;

    public AuthService(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _jwtSecret = configuration["Jwt:Secret"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure";
        _jwtExpirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "24");
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userService.ValidateCredentialsAsync(request.Email, request.Password);
        if (user == null)
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        return new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(_jwtExpirationHours),
            User = user
        };
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