using System.IO;
using KL.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

namespace Game.ModCore
{

    internal static class Detour
    {
        public static LogWriter log;
        private static string _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
        "\\..\\LocalLow\\Kodo Linija\\Stardeus\\Mods\\DetourCoreLib\\config.json";
        private static Config _conf;

        [RuntimeInitializeOnLoadMethod]
        private static void init()
        {
            if (!File.Exists(_path))
            {
                D.Warn("config not found creating");
                _conf = new Config();
                string file = JsonConvert.SerializeObject(_conf);
                File.WriteAllText(_path, file);
            }
            else
            {
                D.Warn("reading confs");
                _conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_path));
            }

            if (_conf.debug_logging == true) log = new LogWriter(_path);

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
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo methodinfo in methods)
                    {
                        InjectAttribute attribute = methodinfo.GetCustomAttribute<InjectAttribute>();
                        if (attribute is null) continue;
                        list.Add(methodinfo);
                    }
                }
            }
            list = chkConflicts(list);
            if (list is List<MethodInfo>)
            {
                injectAttributeall(list);
            }
            else
            {
                D.Err("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                D.Err("injecting stoped due to error!");
                return;
            }
        }
        private static List<MethodInfo> chkConflicts(List<MethodInfo> list)
        {

            bool conflict = false;
            Dictionary<MethodInfo, string> unique = new Dictionary<MethodInfo, string>();
            foreach (MethodInfo item in list)
            {
                if (unique[item] == null) unique[item] = item.ToString();
                else { D.Err("method conflict = " + unique[item]); conflict = true; }
            }
            if (conflict)
            {
                if (_conf.ignore_conflicts) 
                { D.Warn("ignoring conflits set into configs"); return list; }
                list = new List<MethodInfo>();
                foreach (var item in unique) list.Add(item.Key);
            }
            return list;

        }
        private static void injectAttributeall(List<MethodInfo> list)
        {
            List<Exception> errors = new List<Exception>();
            List<string> log = new List<string>();
            foreach (MethodInfo method in list)
            {
                InjectAttribute targ = method.GetCustomAttribute<InjectAttribute>();
                try
                {
                    MethodInfo target = targ.targetType.GetMethod(targ.methodName, targ.methodFlags);
                    if (target == null)
                    {
                        D.Err(String.Format(
                        "Cannot bind to target method! [{0}.{1} {2}]",
                        targ.targetType, targ.methodName, targ.methodFlags
                        ));
                        continue;
                    }
                    DoDetour(target, method);
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
