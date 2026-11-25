using System;
using CEditorBoss.src;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppAssets.Scripts.Unity.Player;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BossType = Il2CppAssets.Scripts.Data.Boss.BossType;

namespace CEditorBoss.LoadBoss;

internal static class EditorBossRun
{
    private const string Marker = " [CEditorBoss]";

    internal static bool FinalTierDefeated { get; set; }

    internal static bool IsActive
    {
        get
        {
            var game = InGameData.CurrentGame;

            return game != null
                && game.gameType == GameType.BossChallenge
                && game.dcModel?.name?.EndsWith(
                    Marker, StringComparison.Ordinal) == true;
        }
    }

    internal static void Launch(
        DailyChallengeModel editor,
        MapSaveDataModel? save = null)
    {
        var challenge = editor.Clone();
        challenge.name = (editor.name ?? "") + Marker;
        challenge.chalType = ChallengeType.BossChallenge;
        challenge.startRules.endRound = Math.Max(140, challenge.startRules.endRound);

        var scoring = LeaderboardScoringType.GameTime;

        if (!editor.ignoreLeastMode)
        {
            if (editor.leastCashUsed >= 0)
                scoring = LeaderboardScoringType.LeastCash;
            else if (editor.leastTiersUsed >= 0)
                scoring = LeaderboardScoringType.LeastTiers;
        }

        BossType boss = Main.BossName;

        InGameData.CreateNewInstance();

        var data = InGameData.Editable;
        data.selectedMap = challenge.map;
        data.selectedDifficulty = challenge.difficulty;
        data.selectedMode = challenge.mode;

        data.SetupBossChallenge(
            "",
            boss,
            Main.Elite,
            Main.Ranked,
            challenge,
            scoring
        );

        UI.instance.LoadGame(null, null, save, false, false);
    }

    internal static void GuardVictory(BossBloonManager manager)
    {
        if (manager.checkForVictory && !FinalTierDefeated && IsActive)
            manager.checkForVictory = false;
    }
}

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.StartNewGame))]
internal static class LaunchEditorBossChallenge
{
    private static bool Prefix(ChallengeEditorPlay __instance)
    {
        var editor = __instance.dcm;

        if (!__instance.isCreationMode || editor == null || editor.numberOfPlayers > 1)
            return true;

        EditorBossRun.Launch(editor);
        return false;
    }
}

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.ContinueClicked))]
internal static class ContinueEditorBossChallenge
{
    private static bool Prefix(ChallengeEditorPlay __instance)
    {
        var editor = __instance.dcm;
        var save = __instance.mapSaveData;

        if (!__instance.isCreationMode || editor == null || editor.numberOfPlayers > 1)
            return true;

        if (save == null || save.gameType != GameType.BossChallenge)
            return true;

        EditorBossRun.Launch(editor, save);
        return false;
    }
}

[HarmonyPatch(typeof(Btd6Player), nameof(Btd6Player.SetBossChallengeRecord))]
internal static class KeepEditorRecordLocal
{
    private static bool Prefix() => !EditorBossRun.IsActive;
}

[HarmonyPatch(typeof(BossBloonManager), nameof(BossBloonManager.Init))]
internal static class ResetFinalTier
{
    private static void Prefix()
    {
        EditorBossRun.FinalTierDefeated = false;
    }
}

[HarmonyPatch(typeof(BossBloonManager), "BloonStartedDeathAnim")]
internal static class DetectTierFiveDeath
{
    private static void Prefix(Bloon __0)
    {
        if (__0 == null || !EditorBossRun.IsActive) return;

        var model = __0.bloonModel;
        if (model == null || !model.isBoss || model.isBossSegment) return;

        if (model.id?.EndsWith("5", StringComparison.Ordinal) == true)
            EditorBossRun.FinalTierDefeated = true;
    }
}

[HarmonyPatch(typeof(BossBloonManager), "BloonDestroyed")]
internal static class CheckVictoryAfterBossDestruction
{
    private static void Postfix(BossBloonManager __instance)
    {
        EditorBossRun.GuardVictory(__instance);
    }
}

[HarmonyPatch(typeof(BossBloonManager), nameof(BossBloonManager.Process))]
internal static class CheckVictoryDuringSimulation
{
    private static void Prefix(BossBloonManager __instance)
    {
        EditorBossRun.GuardVictory(__instance);
    }
}

[HarmonyPatch(typeof(BossBloonManager), nameof(BossBloonManager.OnRoundEnd))]
internal static class CheckVictoryAtRoundEnd
{
    private static void Prefix(BossBloonManager __instance)
    {
        EditorBossRun.GuardVictory(__instance);
    }
}
