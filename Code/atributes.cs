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
        public injectAttribute(Type target,  string name , BindingFlags flags = BindingFlags.Static | BindingFlags.Public)
        {
            targetType = target;
            methodFlags = flags;
            methodName = name;
        }
    }
}