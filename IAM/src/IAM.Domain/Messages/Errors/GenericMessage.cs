using Myce.Response.Messages;

namespace IAM.Domain.Messages.Errors
{
   public class FailedToRecordDataError : ErrorMessage
   {
      public FailedToRecordDataError() : base("FailedToRecordDataError", "Failed to record data.") { }
   }

   public class NotFoundError : ErrorMessage
   {
      public NotFoundError() : base("NotFoundError", "The requested resource was not found.") { }

      public NotFoundError(string entity) : base("NotFoundDetailedError", "{entity} not found.") 
      {
         AddVariable("entity", entity);
      }
   }

   public class UnauthorizedError : ErrorMessage
   {
      public UnauthorizedError() : base("UnauthorizedError", "Invalid email or password.") { }
   }
}
