using IAM.Domain.Enums;

namespace IAM.Domain.DTOs.Responses
{
   public class ParameterLiteDto
   {
      public Guid Id { get; set; }
      public string Group { get; set; }
      public string Key { get; set; }
      public string Name { get; set; }
      public ParameterType Type { get; set; }
      public string Value { get; set; }
   }
}
