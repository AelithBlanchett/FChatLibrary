using System;
using System.Reflection;

namespace FChatLib
{
    class TypeProxy : MarshalByRefObject
    {
        public Type LoadFromAssembly(string assemblyPath, string typeName)
        {
            try
            {
                var asm = Assembly.LoadFile(assemblyPath);
                var type = asm.GetType(typeName);
                return type;
            }
            catch (Exception ex) {
                return null;
            }
        }


        public Assembly GetAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.LoadFile(assemblyPath);
            }
            catch (Exception)
            {
                return null;
                // throw new InvalidOperationException(ex);
            }
        }
    }
}