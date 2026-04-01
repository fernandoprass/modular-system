namespace Shared.Domain.Entities
{
   /// <summary>
   /// The Entity class is an abstract base class that represents a generic entity in the domain model.
   /// It includes common properties such as Id, CreatedAt, UpdatedAt, CreatedBy, and UpdatedBy, 
   /// which are typically used for tracking the lifecycle of entities. The class is designed to be 
   /// inherited by specific entity classes that will define their own properties and behaviors while 
   /// still benefiting from the common functionality provided by this base class.
   /// </summary>
   /// <typeparam name="T">The Type of the Entity</typeparam>
   public abstract class Entity<TId>
   {
      public TId Id { get; set; }
      public DateTime CreatedAt { get; set; }
      public DateTime? UpdatedAt { get; set; }
      public Guid CreatedBy { get; set; }
      public Guid? UpdatedBy { get; set; }
   }

   /// <summary>
   /// The Entity class is a specialization of the generic Entity class for entities that use Guid as their identifier type.
   /// </summary>
   public abstract class Entity : Entity<Guid>
   {
   }
}
