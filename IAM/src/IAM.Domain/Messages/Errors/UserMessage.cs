using Myce.Response.Messages;

namespace IAM.Domain.Messages
{
   public class UserEmailAlreadyExistError : ErrorMessage
   {
      public UserEmailAlreadyExistError(string email)
        : base("USER_EMAIL_ALREADY_EXIST_ERROR", "The email '{email}' already exist.")
      {
         AddVariable("email", email);
      }
   }

   public class UserNotFoundError : ErrorMessage
   {
      public UserNotFoundError()
        : base("USER_NOT_FOUND_ERROR", "User not found.")
      {
      }
   }
}
