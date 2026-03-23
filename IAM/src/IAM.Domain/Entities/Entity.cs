namespace IAM.Domain.Entities
{
   public class Entity
   {
      public Guid Id { get; set; }
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public DateTime? UpdatedAt { get; set; }
   }
}
