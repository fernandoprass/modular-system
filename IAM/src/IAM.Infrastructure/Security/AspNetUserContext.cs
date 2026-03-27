using IAM.Domain;
using IAM.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IAM.Infrastructure.Security;

public class AspNetUserContext : IUserContext
{
   private readonly IHttpContextAccessor _accessor;

   public AspNetUserContext(IHttpContextAccessor accessor)
   {
      _accessor = accessor;
   }

   public Guid CustomerId => GetCustomerId();
   public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
   public bool IsSuperUser => GetIsSuperUser();
   public Guid UserId => GetUserId();

   private Guid GetCustomerId()
   {
      var value = _accessor.HttpContext?.User.FindFirst(Const.Security.Claim.CustomerId)?.Value;
      return Guid.TryParse(value, out var id) ? id : Guid.Empty;
   }

   private bool GetIsSuperUser()
   {
      var value = _accessor.HttpContext?.User.FindFirst(Const.Security.Claim.IsSuperUser)?.Value;
      return Boolean.TryParse(value, out var isSuperUser) ? isSuperUser : false;
   }

   private Guid GetUserId()
   {
      var value = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                  ?? _accessor.HttpContext?.User.FindFirst("sub")?.Value;

      return Guid.TryParse(value, out var id) ? id : Guid.Empty;
   }

   private bool? IsInRole(string role)
   {
      return _accessor.HttpContext?.User.HasClaim(ClaimTypes.Role, role);
   }

}