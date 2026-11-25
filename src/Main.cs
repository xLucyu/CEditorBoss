using CEditorBoss;
using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors.Actions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;

using BossType = Il2CppAssets.Scripts.Data.Boss.BossType;

[assembly: MelonInfo(typeof(CEditorBoss.src.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6-Epic")]

namespace CEditorBoss.src;

public class Main : BloonsTD6Mod
{
    private static bool settingsDirty;

    public static readonly ModSettingEnum<BossType> BossName = new(BossType.Bloonarius)
    {
        displayName = "Boss Type",
        onValueChanged = _ => settingsDirty = true
    };

    public static ModSettingBool Elite = new(false)
    {
        displayName = "Elite",
        description = "Use the Elite boss.",
        onValueChanged = _ => settingsDirty = true
    };

    public static ModSettingBool Ranked = new(false)
    {
        displayName = "Ranked",
        description = "Enable ranked boss mode.",
        onValueChanged = _ => settingsDirty = true
    };

    public static ModSettingDouble BossHealthMultiplier = new(100.0)
    {
        displayName = "Boss Health Multiplier",
        description = "Boss health as a percentage.",
        onValueChanged = _ => settingsDirty = true
    };

    public static ModSettingDouble ShieldHealthMultiplier = new(100.0)
    {
        displayName = "Boss Shield Multiplier",
        description = "Change the Shield HP of Dread and Phayze",
        onValueChanged = _ => settingsDirty = true
    };

    public static ModSettingDouble BossSpeedMultiplier = new(100.0)
    {
        displayName = "Boss Speed Multiplier",
        description = "Boss speed as a percentage.",
        onValueChanged = _ => settingsDirty = true
    };

    public override void OnApplicationStart()
    {
        ModHelper.Msg<Main>("CEditorBoss loaded!");
    }

    public override void OnUpdate()
    {
        if (!settingsDirty) return;

        var editor = Game.instance?.playerService?.Player?.Data?.challengeEditorModel;
        if (editor == null) return;

        BossType boss = BossName;

        editor.roundSets.Clear();
        editor.roundSets.Add(boss.ToString().ToLowerInvariant());

        editor.bloonModifiers.healthMultipliers.boss =
            (float)(BossHealthMultiplier / 100.0);

        editor.bloonModifiers.bossSpeedMultiplier =
            (float)(BossSpeedMultiplier / 100.0);

        editor.startRules.endRound = 140;

        settingsDirty = false;
    }

    public override void OnMatchStart()
    {
        if (!global::CEditorBoss.LoadBoss.EditorBossRun.IsActive) return;
        var model = InGame.Bridge?.Model;
        if (model == null) return;

        var multiplier = (float)(ShieldHealthMultiplier / 100.0);

        foreach (var bloon in model.bloons)
        {
            if (bloon == null || !bloon.isBoss) continue;

            foreach (var armour in bloon.GetBehaviors<GenerateArmourActionModel>())
                armour.amount *= multiplier;

            foreach (var shield in bloon.GetBehaviors<GenerateShieldActionModel>())
                shield.amount *= multiplier;
        }
    }
}
