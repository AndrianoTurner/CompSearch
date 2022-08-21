
using HarmonyLib;
using System.Collections.Generic;

namespace CompSearch
{
    [HarmonyPatch(typeof(PLGlobal), "LoadGXFile")]
    internal class GlobalStart
    {
        public static void Postfix(PLGlobal __instance)
        {
            GeneralInfo generalInfo = new GeneralInfo();

            generalInfo.Name = "Comp. Search";
            generalInfo.Desc = "Use GX [ComponentID] to search for a specific component\n Example: GX 010100  Where 01 - Shield, 01 - Shield Type, 00 - Level";
            generalInfo.ID = __instance.AllGeneralInfos.Count;
            __instance.AllGeneralInfos.Add(generalInfo);
        }
    }


}

