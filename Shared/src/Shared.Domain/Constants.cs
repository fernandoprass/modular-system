namespace Shared.Domain;

public static partial class Const
{
   public static class System
   {
      public const string ModuleName = "Shared";

      public const string DbConnectionName = "SharedDb";
   }
   public static class Entity
   {
      public const string Parameter = nameof(Entities.Parameter);
      public const string ParameterOverride = nameof(Entities.ParameterOverride);
   }
}
