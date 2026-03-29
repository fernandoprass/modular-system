namespace IAM.Domain.Entities
{
   public class ParameterCustomer : Entity
   {
      public string Value { get; set; }
      public Guid CustomerId { get; set; }
      public Guid ParameterId { get; set; }
   }
}
