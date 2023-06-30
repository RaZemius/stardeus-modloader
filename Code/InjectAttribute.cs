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
        public InjectAttribute(Type target, string name, BindingFlags flags = BindingFlags.Public|BindingFlags.Instance|BindingFlags.Static)
        {
            targetType = target;
            methodFlags = flags;
            methodName = name;
        }
    }
}