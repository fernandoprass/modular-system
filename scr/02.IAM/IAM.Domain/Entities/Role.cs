using Shared.Domain.Entities;

namespace IAM.Domain.Entities;

public class Role : Entity
{
   public string Name { get; private set; }
   public Guid? CustomerId { get; private set; } // Roles can be global or specific to a Customer
   public bool IsDefault { get; private set; }

   private readonly List<RoleFeature> _roleFeatures = new();
   public IReadOnlyCollection<RoleFeature> RoleFeatures => _roleFeatures.AsReadOnly();

   private readonly List<UserRole> _userRoles = new();
   public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

   private Role() { }

   public static Role Create(string name, Guid? customerId, bool isDefault = false)
   {
      return new Role
      {
         Id = Guid.CreateVersion7(),
         Name = name,
         CustomerId = customerId,
         IsDefault = isDefault,
         CreatedAt = DateTime.UtcNow
      };
   }

   public void Update(string name)
   {
      Name = name;
      UpdatedAt = DateTime.UtcNow;
   }

   public void AddFeature(Guid featureId)
   {
      if (!_roleFeatures.Any(rf => rf.FeatureId == featureId))
      {
         _roleFeatures.Add(new RoleFeature(Id, featureId));
      }
   }

   public void RemoveFeature(Guid featureId)
   {
       var feature = _roleFeatures.FirstOrDefault(rf => rf.FeatureId == featureId);
       if (feature != null)
       {
           _roleFeatures.Remove(feature);
       }
   }

   public void ClearFeatures() => _roleFeatures.Clear();
}
