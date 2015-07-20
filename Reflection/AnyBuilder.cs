namespace FI.Foundation.Reflection
{
    public class AnyBuilder
    {
        public static object Create(string typeName)
        {
            string assemblyName = null;
            if (typeName.IndexOf(',') >=0)
            {
                assemblyName = typeName.Substring(typeName.IndexOf(',') + 1);
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }

            System.Reflection.Assembly assembly = null;
            if (string.IsNullOrWhiteSpace(assemblyName))
                assembly = System.Reflection.Assembly.GetCallingAssembly();
            else
                assembly = System.Reflection.Assembly.Load(assemblyName);

            if (assembly != null)
            {
                return assembly.CreateInstance(typeName);
            }
            return null;
        }
    }
}
