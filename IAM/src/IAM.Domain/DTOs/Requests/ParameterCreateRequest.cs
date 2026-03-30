using IAM.Domain.Enums;

namespace IAM.Domain.DTOs.Requests
{
   public class ParameterCreateRequest
   {
      public string Key { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public ParameterType Type { get; set; }
      public string Value { get; set; }
      public string ListItems { get; set; }
      public string ExternalListEndpoint { get; set; }
      public bool IsCustomerEditable { get; set; }
      public bool IsVisible { get; set; }
   }
}
