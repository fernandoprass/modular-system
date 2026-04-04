namespace Shared.Domain;

public class SharedParam
{
   private const string Module = "Shared";

   public static class Parameter
   {
      private const string Group = nameof(SharedParam.Parameter);

      public const string OwnerTypesAndPriority = $"{Module}.{Group}.{nameof(OwnerTypesAndPriority)}";
   }
}
