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

    public void Update(string name, string code, string? description, bool isActive)
    {
        Name = name;
        Code = code;
        Description = description;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}