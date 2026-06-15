using System.Collections.Generic;
using MegaCrit.Sts2.Core.Map;

namespace BossEncore;

public static class BossEncoreMapManager
{
    private static readonly Dictionary<ActMap, ExtraNodeChain> _extraNodes = new();
    
    public static void RegisterExtraNodes(ActMap map, ExtraNodeChain chain)
    {
        _extraNodes[map] = chain;
    }
    
    public static ExtraNodeChain? GetExtraNodes(ActMap map)
    {
        return _extraNodes.TryGetValue(map, out var chain) ? chain : null;
    }
    
    public static MapPoint? GetExtraNode(ActMap map, MapCoord coord)
    {
        var chain = GetExtraNodes(map);
        if (chain == null) return null;
        
        if (coord == chain.EventMapPoint.coord) return chain.EventMapPoint;
        if (coord == chain.TreasureMapPoint.coord) return chain.TreasureMapPoint;
        if (coord == chain.RestSiteMapPoint.coord) return chain.RestSiteMapPoint;
        if (coord == chain.ExtraBossMapPoint.coord) return chain.ExtraBossMapPoint;
        
        return null;
    }
    
    public static bool HasExtraNode(ActMap map, MapCoord coord)
    {
        var chain = GetExtraNodes(map);
        if (chain == null) return false;
        
        return coord == chain.EventMapPoint.coord ||
               coord == chain.TreasureMapPoint.coord ||
               coord == chain.RestSiteMapPoint.coord ||
               coord == chain.ExtraBossMapPoint.coord;
    }
}

public class ExtraNodeChain
{
    public MapPoint EventMapPoint { get; }
    public MapPoint TreasureMapPoint { get; }
    public MapPoint RestSiteMapPoint { get; }
    public MapPoint ExtraBossMapPoint { get; }
    
    public ExtraNodeChain(ActMap originalMap)
    {
        int baseRow = originalMap.GetRowCount();
        int centerCol = originalMap.GetColumnCount() / 2;
        
        EventMapPoint = new MapPoint(centerCol, baseRow + 1);
        TreasureMapPoint = new MapPoint(centerCol, baseRow + 2);
        RestSiteMapPoint = new MapPoint(centerCol, baseRow + 3);
        ExtraBossMapPoint = new MapPoint(centerCol, baseRow + 4);
        
        EventMapPoint.PointType = MapPointType.Unknown;
        TreasureMapPoint.PointType = MapPointType.Treasure;
        RestSiteMapPoint.PointType = MapPointType.RestSite;
        ExtraBossMapPoint.PointType = MapPointType.Boss;
        
        originalMap.BossMapPoint.AddChildPoint(EventMapPoint);
        EventMapPoint.AddChildPoint(TreasureMapPoint);
        TreasureMapPoint.AddChildPoint(RestSiteMapPoint);
        RestSiteMapPoint.AddChildPoint(ExtraBossMapPoint);
    }
    
    public IEnumerable<MapPoint> GetAllNodes()
    {
        yield return EventMapPoint;
        yield return TreasureMapPoint;
        yield return RestSiteMapPoint;
        yield return ExtraBossMapPoint;
    }
}