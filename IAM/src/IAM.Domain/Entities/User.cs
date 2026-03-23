namespace IAM.Domain.Entities;
public class User : Entity
{
   //public Guid Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
   public string PasswordHash { get; set; } = string.Empty;
   public bool IsActive { get; set; } = true;
   public bool IsSuperUser { get; set; } = false;
   public Guid CustomerId { get; set; }
   //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
   //public DateTime? UpdatedAt { get; set; }
   public DateTime? EmailVerifiedAt { get; set; }
   public DateTime? LastLoginAt { get; set; }

   public Customer Customer { get; set; } = null!;

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

   private User() { }

   public static User Create(string name, string email, string passwordHash, Guid customerId)
   {
      return new User
      {
         Id = Guid.CreateVersion7(), 
         Name = name,
         Email = email.ToLower().Trim(),
         PasswordHash = passwordHash,
         IsActive = true,
         IsSuperUser = false,
         CreatedAt = DateTime.UtcNow,
         CustomerId = customerId
      };
   }

   public void Update(string name,bool isActive)
   {
      Name = name;
      IsActive = isActive;
      UpdatedAt = DateTime.UtcNow;
   }

   public void UpdatePassword(string newPasswordHash)
   {
      PasswordHash = newPasswordHash;
      UpdatedAt = DateTime.UtcNow;
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