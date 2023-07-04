using System;
using System.Reflection;

namespace Game.ModCore
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public Type targetType;
        public string methodName;
        public BindingFlags methodFlags;
        ///<summary>
        ///this is atribute flag, for injecting methods into the assemblies of game to work with
        ///contains 
        ///Type TargetType, string methodName and BindingFlags methodFlags
        ///
        ///</summary>
        public InjectAttribute(Type target, string name, BindingFlags flags = BindingFlags.Public|BindingFlags.Instance|BindingFlags.Static)
        {
            targetType = target;
            methodFlags = flags;
            methodName = name;
        }
    }
}