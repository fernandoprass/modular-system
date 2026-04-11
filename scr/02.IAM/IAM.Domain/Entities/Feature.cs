using Shared.Domain.Entities;

namespace IAM.Domain.Entities;

public class Feature : EntityAudited
{
   public string Name { get; private set; } // e.g., "User_Create"
   public string Description { get; private set; }
   public string Group { get; private set; } // e.g., "IAM", "Billing"

   private readonly List<RoleFeature> _roleFeatures = new();
   public IReadOnlyCollection<RoleFeature> RoleFeatures => _roleFeatures.AsReadOnly();

   private Feature() { }

   public Feature(string name, string description, string group)
   {
      Id = Guid.CreateVersion7();
      Name = name;
      Description = description;
      Group = group;
      CreatedAt = DateTime.UtcNow;
   }

   public void Update(string description, string group)
   {
      Description = description;
      Group = group;
      UpdatedAt = DateTime.UtcNow;
   }
}
