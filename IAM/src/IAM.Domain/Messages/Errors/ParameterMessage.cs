using Myce.Response.Messages;

namespace IAM.Domain.Messages.Errors
{
   public class DuplicateParameterError : ErrorMessage
   {
      public DuplicateParameterError(string key) 
         : base("DuplicateParameterError", "A parameter with key '{key}' already exists.")
      {
         AddVariable("key", key);
      }
   }

   public class ParameterNotCustomerEditableError : ErrorMessage
   {
      public ParameterNotCustomerEditableError() 
         : base("ParameterNotCustomerEditableError", "This parameter is not editable by customers.") { }
   }

   public class InvalidParameterValueFormatError : ErrorMessage
   {
      public InvalidParameterValueFormatError(string typeName) 
         : base("InvalidParameterValueFormatError", "The value provided is not in a valid format for type '{typeName}'.")
      {
         AddVariable("typeName", typeName);
      }
   }

   public class InvalidParameterKeyFormatError : ErrorMessage
   {
      public InvalidParameterKeyFormatError()
         : base("InvalidParameterKeyFormatError", "Invalid Parameter Key format. The key must follow the pattern 'Module.Group.Name', where each segment contains at least 2 alphanumeric characters.") {}
   }
}
