using System.Reflection;
using HarmonyLib;
using Verse;

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