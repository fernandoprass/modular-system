namespace IAM.Domain.Entities
{
   public class Entity
   {
      public Guid Id { get; set; }
      public DateTime CreatedAt { get; set; }
      public DateTime? UpdatedAt { get; set; }
      public Guid CreatedBy { get; set; }
      public Guid? UpdatedBy { get; set; }
   }
}
