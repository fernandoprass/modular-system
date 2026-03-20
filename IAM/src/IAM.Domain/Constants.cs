namespace IAM.Domain
{
   public static class Const
   {
      public static class Claim
      {
         public const string CustomerId = "customerId";
      }

      public static class Customer
      {
         public const byte RandomCodeSize = 10;
      }

      public static class Entity
      {
         public const string Customer = nameof(Entities.Customer);
         public const string User = nameof(Entities.User);
      }
   }
}
