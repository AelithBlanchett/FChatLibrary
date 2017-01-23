using System;
using System.Reflection;

namespace FChatLib
{
    class TypeProxy : MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.LoadFile(assemblyPath);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}