using Myce.Response.Messages;

namespace IAM.Domain.Messages.Info
{
   public class SuccessInfo : InformationMessage
   {
      public SuccessInfo() : base("SUCCESS", "Operation completed successfully.") { }
   }
}
