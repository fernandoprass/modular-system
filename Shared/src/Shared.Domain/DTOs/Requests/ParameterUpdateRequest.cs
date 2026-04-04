using Shared.Domain.Enums;

namespace Shared.Domain.DTOs.Requests
{
   public class ParameterUpdateRequest
   {
      public string Module { get; set; }
      public string Group { get; set; }
      public string Name { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public ParameterType Type { get; set; }
      public string Value { get; set; }
      public string? ValidationRegex { get; set; }
      public string? ValidationErrorCustomMessage { get; set; }
      public string? ListItems { get; set; }
      public string? ExternalListEndpoint { get; set; }
      public bool IsOwnerEditable { get; set; }
      public string? AllowedOverrideTypes { get; set; }
      public bool IsVisible { get; set; }
   }
}
