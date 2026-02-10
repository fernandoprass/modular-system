namespace IAM.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public bool Type { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; } = new List<User>();
}