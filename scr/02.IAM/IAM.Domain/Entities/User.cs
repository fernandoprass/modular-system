using Shared.Domain.Entities;

namespace IAM.Domain.Entities;
public class User : Entity
{
   public string Name { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
   public string PasswordHash { get; set; } = string.Empty;
   public bool IsActive { get; set; } = true;
   public bool IsSystemAdmin { get; set; } = false;
   public Guid CustomerId { get; set; }
   public DateTime? EmailVerifiedAt { get; set; }
   public DateTime? LastLoginAt { get; set; }
   public DateTime? PasswordExpiresAt { get; set; }
   public Customer Customer { get; set; } = null!;

   private readonly List<UserRole> _userRoles = new();
   public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

   private User() { }

   public static User Create(string name, string email, string passwordHash, Guid customerId)
   {
      var id = Guid.CreateVersion7();
      return new User
      {
         Id = Guid.CreateVersion7(),
         Name = name,
         Email = email.ToLower().Trim(),
         PasswordHash = passwordHash,
         PasswordExpiresAt = DateTime.UtcNow.AddDays(IamConst.Security.User.PasswordExpireTime),
         IsActive = true,
         IsSystemAdmin = false,
         CustomerId = customerId
      };
   }

   public void Update(string name, bool isActive)
   {
      Name = name;
      IsActive = isActive;
   }

   public void UpdatePassword(string newPasswordHash)
   {
      PasswordHash = newPasswordHash;
      UpdatedAt = DateTime.UtcNow;
      PasswordExpiresAt = DateTime.UtcNow.AddDays(IamConst.Security.User.PasswordExpireTime);
   }

   public void UpdateLastLogin()
   {
      LastLoginAt = DateTime.UtcNow;
   }

   public void AddRole(Guid roleId)
   {
      if (!_userRoles.Any(ur => ur.RoleId == roleId))
      {
         _userRoles.Add(new UserRole(Id, roleId));
      }
   }

   public void RemoveRole(Guid roleId)
   {
      var role = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
      if (role != null)
      {
         _userRoles.Remove(role);
      }
   }

   public void ClearRoles() => _userRoles.Clear();
}