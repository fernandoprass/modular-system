namespace IAM.Domain
{
   public static class Const
   {
      public static class Customer
      {
         public const byte RandomCodeSize = 10;
      }

      public static class Entity
      {
         public const string Customer = nameof(Entities.Customer);
         public const string User = nameof(Entities.User);
         public const string Role = nameof(Entities.Role);
         public const string Feature = nameof(Entities.Feature);
      }

      public class Security
      {
         public static class Claim
         {
            public const string CustomerId = "customerId";
            public const string Issuer = "IAM.API";
            public const string Audience = "IAM.Client";
         }
      }
   }
}
