using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace BossEncore.Patches;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]
static class CreateMapPatch
{
    [HarmonyPostfix]
    static void AddExtraNodes(ActMap __result, RunState runState, bool replaceTreasureWithElites)
    {
        if (__result == null) return;
        
        BossEncoreMain.Logger.Info($"Creating extra node chain for act {runState.CurrentActIndex}");
        
        var chain = new ExtraNodeChain(__result);
        BossEncoreMapManager.RegisterExtraNodes(__result, chain);
        
        SetupExtraBossEncounter(runState, chain);
    }
    
    static void SetupExtraBossEncounter(RunState runState, ExtraNodeChain chain)
    {
        var act = runState.Act;
        var rng = new Rng(runState.Rng.Seed, $"boss_encore_{runState.CurrentActIndex}");
        
        var currentBossId = act.BossEncounter?.Id;
        if (currentBossId == null) return;
        
        var availableBosses = act.AllBossEncounters
            .Where(e => e.Id != currentBossId)
            .ToList();
        
        if (runState.CurrentActIndex == 2 && availableBosses.Count > 1)
        {
            var previousBossIds = new HashSet<ModelId>();
            for (int i = 0; i < runState.CurrentActIndex; i++)
            {
                if (i < runState.Acts.Count && runState.Acts[i].BossEncounter?.Id != null)
                {
                    previousBossIds.Add(runState.Acts[i].BossEncounter.Id);
                }
            }
            availableBosses = availableBosses
                .Where(e => !previousBossIds.Contains(e.Id))
                .ToList();
        }
        
        if (availableBosses.Any())
        {
            var extraBoss = rng.NextItem(availableBosses);
            act.SetSecondBossEncounter(extraBoss);
            BossEncoreMain.Logger.Info($"Extra boss set: {extraBoss.Id}");
        }
    }
}