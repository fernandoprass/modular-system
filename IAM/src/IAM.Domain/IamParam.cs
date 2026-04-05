namespace IAM.Domain
{
   public static class IamParam
   {
      private const string Module = "IAM";

      public static class Security
      {
         private const string Group = "Security";

         public const string PasswordExpireTime = $"{Module}.{Group}.{nameof(PasswordExpireTime)}";
      }
   }
}
