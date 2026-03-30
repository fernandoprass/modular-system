namespace IAM.Domain
{
   public static class Param
   {
      public static class IAM
      {
         private const string Module = "IAM";

         public static class Security
         {
            private const string Group = "Security";

            public const string PasswordExpireTime = $"{Module}.{Group}.PasswordExpireTime";
         }
      }
   }
}
