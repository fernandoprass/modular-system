namespace IAM.Domain
{
   public class ParameterKey
   {
      public string Module { get; }
      public string Group { get; }
      public string Name { get; }
      public string Key { get; }

      public ParameterKey(string key)
      {
         var parts = key.Split('.');
         if (parts.Length != 3)
            throw new ArgumentException("The parameter Key must follow the format 'Module.Group.Key'");

         Module = parts[0];
         Group = parts[1];
         Name = parts[2];
         Key = key;
      }

      public static implicit operator ParameterKey(string path) => new(path);
   }
}
