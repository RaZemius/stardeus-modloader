using Game.Components;
using Game.Constants;
using Game.Rendering;
using Game.UI;
using KL.Randomness;
using KL.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.ModCore
{

    internal static class Detour
    {
        private static FieldInfo coroutineField = typeof(Coroutines).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
        private static string AssemblyName => ((IEnumerable<string>)Assembly.GetAssembly(typeof(Detour)).FullName.Split(',')).First<string>();
        private static Coroutines GetCoroutines() => (Coroutines)Detour.coroutineField.GetValue((object)null);
        /*
        public void addfield(FieldInfo info){field.Add(info);}
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class AddObject : Attribute{
            public AddObjectAttribute() => ;
        }
        */
        [RuntimeInitializeOnLoadMethod]
        private static void init()
        {
            D.Warn("making trigger for detour");
            Action init = delegate () { Detour.runcheck(); };
            Ready.WhenCore(init);
        }
        public static void runcheck()
        {
            List<MethodInfo> list = new List<MethodInfo>();
            D.Warn("triggered check. running");
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly code in assemblies)
            {
                Type[] types = code.GetTypes();
                foreach (Type type in types)
                {
                    MethodInfo[] methods = type.GetMethods();
                    foreach (MethodInfo methodinfo in methods)
                    {
                        InjectAttribute attribute = methodinfo.GetCustomAttribute<InjectAttribute>();
                        if (attribute == null) continue;
                        try
                        {
                            D.Warn("detected class target = " +
                            attribute.targetType.ToString() +
                            "." +
                            attribute.methodName);
                            //methodinfo.Invoke(null, null);
                            list.Add(methodinfo);
                        }
                        catch
                        {
                            D.Err("failed to invoke on runtime. mod method" + methodinfo.ToString());
                        }

                    }
                }


            }
            foreach (MethodInfo item in list)
            {
                D.Warn(item.ToString());
            }
            injectAttributeall(list);
        }
        private static void injectAttributeall(List<MethodInfo> list)
        {
            List<Exception> errors = new List<Exception>();
            List<string> log = new List<string>();
            foreach (MethodInfo method in list)
            {
                InjectAttribute targ =
                ((InjectAttribute)method.GetCustomAttribute(typeof(InjectAttribute)));
                try
                {
                    MethodInfo target = targ.targetType.GetMethod(targ.methodName, targ.methodFlags);
                    if (target == null)
                    {
                        D.Err(String.Format("Cannot find target method! [{0}.{1} {2}]", targ.targetType, targ.methodName, targ.methodFlags));
                        continue;
                    }
                    DoDetour(target,method);
                }
                catch (Exception ex)
                {
                    log.Add(targ.methodName);
                    errors.Add(ex);
                }
            }
            if (log.Count > 0)
            {
                string text = "<!! cathed seg fault during init of mods !!>\n";
                foreach (string item in log)
                {
                    text += item + "\n";
                }
                foreach (Exception ex in errors)
                {
                    text += ex.ToString();
                }
                text += "end of error log";
                D.Err(text);
            }
            else
            {
                D.Warn("detour complited without issues");
            }

        }
        /*
                private static bool DoInjectAtinjectAttribute()
                {
                    List<MethodInfo> methodInfoList1 = new List<MethodInfo>();
                    List<MethodInfo> methodInfoList2 = new List<MethodInfo>();
                    BindingFlags[] bindall = new BindingFlags[3] {
                        BindingFlags.Instance | BindingFlags.Public,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic};
                    System.Type type1 = typeof(CrafterComp);
                    System.Type type2 = typeof(CrafterCompModding);
                    MethodInfo[] methods = type1.GetMethods();
                    string[] list = new string[] { "OnLateReady", "OnSave", "OnLoad", "RebuildIngredientsReq", "ProvideRequestedMat", "SpawnCraftable", "SwitchToCrafting", "StopProducing" };
                    foreach (string name in list)
                    {
                        foreach (BindingFlags attr in bindall)
                        {
                            var add2 = type2.GetMethod(name, attr);
                            var add1 = type1.GetMethod(name, attr);
                            if (add1 is not null) methodInfoList1.Add(add1);
                            if (add2 is not null) methodInfoList2.Add(add2);
                        }
                    }

                    Debug.GameStateDebug.print(methodInfoList1);

                    methodInfoList1.Add(type1.GetMethod("OnLateReady", bindingAttr1));
                    methodInfoList2.Add(type2.GetMethod("OnLateReady", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("OnSave", bindingAttr1));
                    methodInfoList2.Add(type2.GetMethod("OnSave", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("OnLoad", bindingAttr2));
                    methodInfoList2.Add(type2.GetMethod("OnLoad", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("RebuildIngredientsReq", bindingAttr2));
                    methodInfoList2.Add(type2.GetMethod("RebuildIngredientsReq", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("ProvideRequestedMat", bindingAttr1));
                    methodInfoList2.Add(type2.GetMethod("ProvideRequestedMat", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("SpawnCraftable", bindingAttr2));
                    methodInfoList2.Add(type2.GetMethod("SpawnCraftable", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("SwitchToCrafting", bindingAttr1));
                    methodInfoList2.Add(type2.GetMethod("SwitchToCrafting", bindingAttr3));
                    methodInfoList1.Add(type1.GetMethod("StopProducing", bindingAttr1));
                    methodInfoList2.Add(type2.GetMethod("StopProducing", bindingAttr3));

                    if (methodInfoList1.Count != methodInfoList2.Count)
                        return false;
                    for (int index = 0; index < methodInfoList1.Count; ++index)
                    {
                        MethodInfo source = methodInfoList1[index];
                        MethodInfo destination = methodInfoList2[index];
                        if (source == (MethodInfo)null || destination == (MethodInfo)null)
                            return false;
                        Detour.DoDetour(source, destination);
                    }
                    return true;
                }
        */

        public static unsafe void DoDetour(MethodInfo source, MethodInfo destination)
        {
            if (IntPtr.Size == 8)
            {
                RuntimeMethodHandle methodHandle = source.MethodHandle;
                IntPtr functionPointer = methodHandle.GetFunctionPointer();
                byte* int64_1 = (byte*)functionPointer.ToInt64();
                methodHandle = destination.MethodHandle;
                functionPointer = methodHandle.GetFunctionPointer();
                long int64_2 = functionPointer.ToInt64();
                byte* numPtr1 = int64_1;
                long* numPtr2 = (long*)(numPtr1 + 2);
                *numPtr1 = (byte)72;
                numPtr1[1] = (byte)184;
                *numPtr2 = int64_2;
                numPtr1[10] = byte.MaxValue;
                numPtr1[11] = (byte)224;
            }
            else
            {
                RuntimeMethodHandle methodHandle = source.MethodHandle;
                IntPtr functionPointer = methodHandle.GetFunctionPointer();
                int int32_1 = functionPointer.ToInt32();
                methodHandle = destination.MethodHandle;
                functionPointer = methodHandle.GetFunctionPointer();
                int int32_2 = functionPointer.ToInt32();
                byte* numPtr3 = (byte*)int32_1;
                int* numPtr4 = (int*)(numPtr3 + 1);
                int num = int32_2 - int32_1 - 5;
                *numPtr3 = (byte)233;
                *numPtr4 = num;
            }
        }
    }
}
