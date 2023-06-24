
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
using Game;

namespace Game.ModCore{
    
    [AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class injectAttribute : Attribute{
        public bool test;
        public injectAttribute() => test = true;
    }
}