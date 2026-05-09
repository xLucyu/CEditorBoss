using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;


namespace CEditorBoss.LoadBoss;

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.StartNewGame))]
public static class StartNewGame
{
    private static void Prefix()
    {
        LoadBoss.Prefix();
    }
}

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.ContinueClicked))]
public static class ContinueBtn
{
    private static void Prefix()
    {
        LoadBoss.Prefix();
    }
}

public static class LoadBoss
{
    private static Il2CppAssets.Scripts.Data.Boss.BossType ConvertToGameBossType(CEditorBoss.BossType bossType)
    {
        return bossType switch
        {
            CEditorBoss.BossType.Bloonarius => Il2CppAssets.Scripts.Data.Boss.BossType.Bloonarius,
            CEditorBoss.BossType.Lych => Il2CppAssets.Scripts.Data.Boss.BossType.Lych,
            CEditorBoss.BossType.Vortex => Il2CppAssets.Scripts.Data.Boss.BossType.Vortex,
            CEditorBoss.BossType.Dreadbloon => Il2CppAssets.Scripts.Data.Boss.BossType.Dreadbloon,
            CEditorBoss.BossType.Phayze => Il2CppAssets.Scripts.Data.Boss.BossType.Phayze,
            CEditorBoss.BossType.Blastapopoulus => Il2CppAssets.Scripts.Data.Boss.BossType.Blastapopoulos,
            CEditorBoss.BossType.Diamondback => Il2CppAssets.Scripts.Data.Boss.BossType.Diamondback,
            _ => Il2CppAssets.Scripts.Data.Boss.BossType.Bloonarius
        };
    }

    public static void Prefix()
    {
        var bossType = ConvertToGameBossType(Main.BossName);
        var isElite = Main.Elite;
        var isRanked = Main.Ranked;

        InGameData.Editable.SetupBoss(
           bossType.ToString(),
           bossType,
           isElite,
           isRanked,
           BossGameData.DefaultSpawnRounds,
           new DailyChallengeModel
           {
               difficulty = InGameData.Editable.selectedDifficulty,
               map = InGameData.Editable.selectedMap,
               mode = InGameData.Editable.selectedMode,
               towers = new TowerData[]
                    {
                          new() { isHero = true, tower = DailyChallengeModel.CHOSENPRIMARYHERO, max = 1 }
                   }.ToIl2CppList(),
           },
                LeaderboardScoringType.GameTime
        );
    }
}