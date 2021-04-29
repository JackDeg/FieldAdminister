using System.Reflection;
using HarmonyLib;
using Verse;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;

// Modified from the same named file from Combat Extended, under CC-BY-NC-SA-4.0.

namespace JackDeg_FieldAdminister
{
    [StaticConstructorOnStartup]
    public static class HarmonyBase
    {
        static HarmonyBase()
        {
            var harmonyInstance = new Harmony("FieldAdminister");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[FieldAdminister] inited");
        }
    }
}