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
   public bool IsSystemAdmin => GetIsSystemAdmin();
   public Guid UserId => GetUserId();

   private Guid GetCustomerId()
   {
      var value = _accessor.HttpContext?.User.FindFirst(Const.Security.Claim.CustomerId)?.Value;
      return Guid.TryParse(value, out var id) ? id : Guid.Empty;
   }

   private bool GetIsSystemAdmin()
   {
      var value = _accessor.HttpContext?.User.FindFirst(Const.Security.Claim.IsSystemAdmin)?.Value;
      return Boolean.TryParse(value, out var isSystemAdmin) ? isSystemAdmin : false;
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