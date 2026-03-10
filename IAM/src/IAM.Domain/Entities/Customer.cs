namespace IAM.Domain.Entities;

public class Customer
{
   public Guid Id { get; set; }
   public CustomerType Type { get; set; }
   public string Code { get; set; }
   public string Name { get; set; }
   public string? Description { get; set; }
   public bool IsActive { get; set; } = true;
   public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
   public DateTime? UpdatedAt { get; set; }

   // Navigation property
   public ICollection<User> Users { get; set; } = new List<User>();

   public static Customer Create(CustomerType type, string code, string name, string? description)
   {
      return new Customer
      {
         Id = Guid.CreateVersion7(),
         Type = type,
         Code = code,
         Name = name,
         Description = description,
         IsActive = true,
         CreatedAt = DateTime.UtcNow
      };
   }

   public void Update(string code, string name, string? description, bool isActive)
   {
      Code = code;
      Name = name;
      Description = description;
      IsActive = isActive;
      UpdatedAt = DateTime.UtcNow;
   }
}