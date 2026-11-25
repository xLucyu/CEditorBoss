using CEditorBoss.src;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

using BossType = Il2CppAssets.Scripts.Data.Boss.BossType;

namespace CEditorBoss.LoadBoss;

[HarmonyPatch(typeof(InGame), nameof(InGame.MapDataSaveId), MethodType.Getter)]
internal static class EditorBossMapSaveSlot
{
    private static void Postfix(ref string __result)
    {
        if (!EditorBossRun.IsActive) return;

        BossType boss = Main.BossName;
        var editorSlot = DailyChallengeExtensions.GetMapSaveId(
            ChallengeType.UserCreate,
            false,
            new(boss)
        );

        if (!string.IsNullOrEmpty(editorSlot))
            __result = editorSlot;
    }
}

[HarmonyPatch(typeof(InGame), "CreateCurrentMapSave")]
internal static class MarkEditorBossSave
{
    private static void Postfix(MapSaveDataModel __result)
    {
        if (__result == null || !EditorBossRun.IsActive) return;

        __result.isCreationMode = true;
    }
}
