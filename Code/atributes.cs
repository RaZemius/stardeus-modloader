using System;
using System.Reflection;

namespace Game.ModCore
{

    [AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class injectAttribute : Attribute
    {
        
        public Type targetType;
        public string methodName;
        public BindingFlags methodFlags;
        public bool test;
        public injectAttribute(Type target, BindingFlags flags = BindingFlags.Static | BindingFlags.Public, string name = "" )
        {
            targetType = target;
            methodFlags = flags;
            methodName = name;
        }
    }
}