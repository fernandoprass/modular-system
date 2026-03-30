using IAM.Domain.Enums;

namespace IAM.Domain.DTOs.Requests
{
   public class ParameterSearchRequest
   {
      public string Module { get; set; }
      public string Group { get; set; }
      public string Name { get; set; }
      public string Key { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
   }
}
