using IAM.Domain.Enums;

namespace IAM.Domain.Entities
{
   public class Parameter : Entity
   {
      public string Module { get; private set; }
      public string Group { get; private set; }
      public string Name { get; private set; }
      public string Key { get; private set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public ParameterType Type { get; set; }
      public string Value { get; set; }
      public string? ListItems { get; set; }
      public string? ExternalListEndpoint { get; set; }
      public bool IsCustomerEditable { get; set; }
      public bool IsVisible { get; set; }

      public ICollection<ParameterCustomer> ParameterCustomers { get; set; } = new List<ParameterCustomer>();

      public static Parameter Create(
        string module,
        string group,
        string name,
        string title,
        string description,
        ParameterType type,
        string value,
        string? listItems = null,
        string? externalListEndpoint = null,
        bool isCustomerEditable = false,
        bool isVisible = true)
      {
         var parameter = new Parameter
         {
            Id = Guid.CreateVersion7(),
            Module = module,
            Group = group,
            Name = name,
            Key = GetKey(module, group, name),
            Title = title,
            Description = description,
            Type = type,
            Value = value,
            ListItems = listItems,
            ExternalListEndpoint = externalListEndpoint,
            IsCustomerEditable = isCustomerEditable,
            IsVisible = isVisible
         };

         return parameter;
      }

      public void Update(
        string module,
        string group,
        string name,
        string title,
        string description,
        ParameterType type,
        string value,
        string? listItems = null,
        string? externalListEndpoint = null,
        bool isCustomerEditable = false,
        bool isVisible = true)
      {
         Module = module;
         Group = group;
         Name = name;
         Key = GetKey(module, group, name);
         Title = title;
         Description = description;
         Type = type;
         Value = value;
         ListItems = listItems;
         ExternalListEndpoint = externalListEndpoint;
         IsCustomerEditable = isCustomerEditable;
         IsVisible = isVisible;
      }

      private static string GetKey(string module, string group, string name)
      {
         return $"{module}.{group}.{name}";
      }
   }
}
