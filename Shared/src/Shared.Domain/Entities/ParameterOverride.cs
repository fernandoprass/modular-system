namespace Shared.Domain.Entities
{
   public class ParameterOverride : Entity
   {
      public string OwnerType { get; set; }
      public string Value { get; set; }
      public Guid OwnerId { get; set; }
      public Guid ParameterId { get; set; }

      public static ParameterOverride Create(Guid parameterId, string ownerType, Guid ownerId, string value)
      {
         return new ParameterOverride
         {
            Id = Guid.CreateVersion7(),
            ParameterId = parameterId,
            OwnerType = ownerType,
            OwnerId = ownerId,
            Value = value
         };
      }

      public void Update(string value)
      {
            Value = value;
      }
   }
}
