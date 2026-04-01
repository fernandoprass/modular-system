namespace Shared.Domain.Entities
{
   public class ParameterCustomer : Entity
   {
      public string Value { get; set; }
      public Guid CustomerId { get; set; }
      public Guid ParameterId { get; set; }

      public static ParameterCustomer Create(Guid parameterId, Guid customerId, string value)
      {
         return new ParameterCustomer
         {
            Id = Guid.CreateVersion7(),
            ParameterId = parameterId,
            CustomerId = customerId,
            Value = value
         };
      }

      public void Update(string value)
      {
            Value = value;
      }
   }
}
