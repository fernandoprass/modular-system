using Myce.Response.Messages;

namespace IAM.Domain.Messages.Errors;

public class DuplicateCodeError : ErrorMessage
{
    public DuplicateCodeError(string code)
        : base("DuplicateCodeError", "The customer code '{code}' already exists.")
    {
        AddVariable("code", code);
    }
}

public class InvalidCodeFormatError : ErrorMessage
{
    public InvalidCodeFormatError()
        : base("InvalidCodeFormatError", "For Person customers, the code must be exactly 10 alphanumeric digits.")
    {
    }
}
