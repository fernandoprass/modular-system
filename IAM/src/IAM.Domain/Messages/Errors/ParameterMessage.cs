using Myce.Response.Messages;

namespace IAM.Domain.Messages.Errors
{
   public class DuplicateParameterError : ErrorMessage
   {
      public DuplicateParameterError(string group, string key) 
         : base("DuplicateParameterError", "A parameter with Group '{group}' and Key '{key}' already exists.")
      {
         AddVariable("group", group);
         AddVariable("key", key);
      }
   }

   public class ParameterNotCustomerEditableError : ErrorMessage
   {
      public ParameterNotCustomerEditableError() 
         : base("ParameterNotCustomerEditableError", "This parameter is not editable by customers.") { }
   }

   public class InvalidParameterFormatError : ErrorMessage
   {
      public InvalidParameterFormatError(string typeName) 
         : base("InvalidParameterFormatError", "The value provided is not in a valid format for type '{typeName}'.")
      {
         AddVariable("typeName", typeName);
      }
   }
}
