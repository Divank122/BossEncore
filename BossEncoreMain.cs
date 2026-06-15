using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace BossEncore;

[ModInitializer(nameof(Initialize))]
public static class BossEncoreMain
{
    public static Logger Logger => new("BossEncore", LogType.Generic);
    
    public const string ModId = "BossEncore";
    
    public static void Initialize()
    {
        Logger.Info("Boss Encore initializing...");
        
        var harmony = new Harmony(ModId);
        harmony.PatchAll();
        
        Logger.Info("Boss Encore initialized successfully!");
    }
}