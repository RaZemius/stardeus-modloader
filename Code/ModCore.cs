
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

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class Detour : Attribute
    {
        private static FieldInfo coroutineField = typeof(Coroutines).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
        private static string AssemblyName => ((IEnumerable<string>)Assembly.GetAssembly(typeof(Detour)).FullName.Split(',')).First<string>();
        public static void Info(string text) => Detour.GetCoroutines().StartCoroutine(Detour.DelayedPopup("Icons/Color/Info", T.Information, text));
        public static void Warning(string text) => Detour.GetCoroutines().StartCoroutine(Detour.DelayedPopup("Icons/Color/Warning", T.Warning, text));
        private static Coroutines GetCoroutines() => (Coroutines)Detour.coroutineField.GetValue((object)null);
        public static IEnumerator DelayedPopup(string icon, string title, string text)
        {
            while (!RenderingService.Sprites.Contains(icon))
                yield return (object)new WaitForSecondsRealtime(1f);
            yield return (object)new WaitForSecondsRealtime(Rng.URange(0.5f, 1f));
            UIPopupWidget.Spawn(icon, title, text);
        }
        private List<FieldInfo> field = new List<FieldInfo>();
        private List<MethodInfo> methods = new List<MethodInfo>();

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class AddObjectToDetour : Attribute{
        }
        //[RuntimeInitializeOnLoadMethod]
        //private static void init() => Detour.Inject();

        public static void Inject()
        {
            try
            {
                if (!Detour.DoInject())
                    Detour.Warning("[" + Detour.AssemblyName + "] Failed to inject properly: method not found");
                else
                    Detour.Info("[" + Detour.AssemblyName + "] Detour complete.");
            }
            catch (Exception ex)
            {
                D.Err("[" + Detour.AssemblyName + "] Injection failed!");
                D.Err(ex.StackTrace);
                Detour.Warning("[" + Detour.AssemblyName + "] Failed to inject properly, see Player.log for error message.");
            }
        }

        private static bool DoInject()
        {
            List<MethodInfo> methodInfoList1 = new List<MethodInfo>();
            List<MethodInfo> methodInfoList2 = new List<MethodInfo>();
            BindingFlags bindingAttr1 = BindingFlags.Instance | BindingFlags.Public;
            BindingFlags bindingAttr2 = BindingFlags.Instance | BindingFlags.NonPublic;
            BindingFlags bindingAttr3 = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            System.Type type1 = typeof(CrafterComp);
            System.Type type2 = typeof(CrafterCompModding);
            MethodInfo[] methods = type1.GetMethods();
            string[] list = new string[] { "OnLateReady", "OnSave", "OnLoad", "RebuildIngredientsReq", "ProvideRequestedMat", "SpawnCraftable", "SwitchToCrafting", "StopProducing" };
            foreach (string name in list)
            {
                methodInfoList1.Add(type1.GetMethod(name, bindingAttr1));
                methodInfoList2.Add(type2.GetMethod(name, bindingAttr3));
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
