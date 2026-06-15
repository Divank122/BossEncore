using HarmonyLib;
using MegaCrit.Sts2.Core.Map;

namespace BossEncore.Patches;

[HarmonyPatch(typeof(ActMap), nameof(ActMap.GetPoint), typeof(MapCoord))]
static class GetPointPatch
{
    [HarmonyPrefix]
    static bool CheckExtraNodes(ActMap __instance, MapCoord coord, ref MapPoint? __result)
    {
        var extraNode = BossEncoreMapManager.GetExtraNode(__instance, coord);
        if (extraNode != null)
        {
            __result = extraNode;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(ActMap), nameof(ActMap.HasPoint))]
static class HasPointPatch
{
    [HarmonyPrefix]
    static bool CheckExtraNodes(ActMap __instance, MapCoord coord, ref bool __result)
    {
        if (BossEncoreMapManager.HasExtraNode(__instance, coord))
        {
            __result = true;
            return false;
        }
        return true;
    }
}