namespace IAM.Domain.Entities;

public class RoleFeature
{
   public Guid RoleId { get; private set; }
   public Guid FeatureId { get; private set; }
   public Role Role { get; private set; } = null!;
   public Feature Feature { get; private set; } = null!;

   private RoleFeature() { }

   public RoleFeature(Guid roleId, Guid featureId)
   {
      RoleId = roleId;
      FeatureId = featureId;
   }
}
