namespace Shared.Domain.DTOs.Requests
{
   public class ParameterOwnerUpdateRequest
   {
      public string OwnerType { get; set; }
      public string Value { get; set; }
      public Guid OwnerId { get; set; }
   }
}
