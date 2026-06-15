using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace BossEncore.Patches;

[HarmonyPatch(typeof(NMapScreen), nameof(NMapScreen.SetMap))]
static class MapScreenPatch
{
    private static readonly AccessTools.FieldRef<NMapScreen, Control> _pointsRef = 
        AccessTools.FieldRefAccess<NMapScreen, Control>("_points");
    
    private static readonly AccessTools.FieldRef<NMapScreen, Dictionary<MapCoord, NMapPoint>> _mapPointDictionaryRef = 
        AccessTools.FieldRefAccess<NMapScreen, Dictionary<MapCoord, NMapPoint>>("_mapPointDictionary");
    
    private static readonly AccessTools.FieldRef<NMapScreen, RunState> _runStateRef = 
        AccessTools.FieldRefAccess<NMapScreen, RunState>("_runState");
    
    [HarmonyPostfix]
    static void RenderExtraNodes(NMapScreen __instance, ActMap map)
    {
        var chain = BossEncoreMapManager.GetExtraNodes(map);
        if (chain == null) return;
        
        BossEncoreMain.Logger.Info("Rendering extra nodes on map screen");
        
        float baseY = -1980f;
        float spacing = 300f;
        float centerX = -200f;
        
        var points = _pointsRef(__instance);
        var dict = _mapPointDictionaryRef(__instance);
        var runState = _runStateRef(__instance);
        
        RenderExtraNode(__instance, points, dict, runState, chain.EventMapPoint, centerX, baseY - spacing);
        RenderExtraNode(__instance, points, dict, runState, chain.TreasureMapPoint, centerX, baseY - 2 * spacing);
        RenderExtraNode(__instance, points, dict, runState, chain.RestSiteMapPoint, centerX, baseY - 3 * spacing);
        RenderExtraNode(__instance, points, dict, runState, chain.ExtraBossMapPoint, centerX, baseY - 4 * spacing);
        
        DrawPaths(__instance, dict, chain);
    }
    
    static void RenderExtraNode(NMapScreen screen, Control points, Dictionary<MapCoord, NMapPoint> dict, 
        RunState runState, MapPoint point, float x, float y)
    {
        var node = NNormalMapPoint.Create(point, screen, runState);
        node.Position = new Godot.Vector2(x, y);
        
        points.AddChild(node);
        dict[point.coord] = node;
    }
    
    static void DrawPaths(NMapScreen screen, Dictionary<MapCoord, NMapPoint> dict, ExtraNodeChain chain)
    {
        DrawPath(screen, dict, chain.EventMapPoint.parents.FirstOrDefault(), chain.EventMapPoint);
        DrawPath(screen, dict, chain.EventMapPoint, chain.TreasureMapPoint);
        DrawPath(screen, dict, chain.TreasureMapPoint, chain.RestSiteMapPoint);
        DrawPath(screen, dict, chain.RestSiteMapPoint, chain.ExtraBossMapPoint);
    }
    
    static void DrawPath(NMapScreen screen, Dictionary<MapCoord, NMapPoint> dict, MapPoint? from, MapPoint to)
    {
        if (from == null) return;
        
        if (!dict.TryGetValue(from.coord, out var fromNode) ||
            !dict.TryGetValue(to.coord, out var toNode))
        {
            return;
        }
        
        var start = fromNode.Position;
        var end = toNode.Position;
        
        CreatePath(screen, start, end);
    }
    
    static void CreatePath(NMapScreen screen, Godot.Vector2 start, Godot.Vector2 end)
    {
        var method = AccessTools.Method(typeof(NMapScreen), "CreatePath");
        method.Invoke(screen, new object[] { start, end });
    }
}