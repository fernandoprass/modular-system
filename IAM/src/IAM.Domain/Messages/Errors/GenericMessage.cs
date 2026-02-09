using Myce.Response.Messages;

namespace IAM.Domain.Messages.Errors
{
   public class FailedToRecordDataError : ErrorMessage
   {
      public FailedToRecordDataError() : base("FAILED_TO_RECORD_DATA_ERROR", "Failed to record data.") { }
   }

   public class NotFoundError : ErrorMessage
   {
      public NotFoundError() : base("NOT_FOUND", "The requested resource was not found.") { }
   }
}
