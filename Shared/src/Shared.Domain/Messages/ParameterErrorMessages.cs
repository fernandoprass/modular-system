using Myce.Response.Messages;

namespace Shared.Domain.Messages;
public class DuplicateParameterError : ErrorMessage
{
   public DuplicateParameterError(string module, string group, string name)
      : base("DuplicateParameterError", "A parameter with Module '{module}', Group '{group}' and Name '{name}' already exists.")
   {
      AddVariable(nameof(module), module);
      AddVariable(nameof(group), group);
      AddVariable(nameof(name), name);
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
      AddVariable(nameof(typeName), typeName);
   }
}

public class InvalidParameterKeyFormatError : ErrorMessage
{
   public InvalidParameterKeyFormatError()
      : base("InvalidParameterKeyFormatError", "Invalid Parameter Key format. The key must follow the pattern 'Module.Group.Name', where each segment contains at least 2 alphanumeric characters.") {}
}
