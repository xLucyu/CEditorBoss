using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using CEditorBoss;
using Il2CppAssets.Scripts.Unity;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

[assembly: MelonInfo(typeof(CEditorBoss.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CEditorBoss;

public enum BossType
{
    Bloonarius,
    Vortex,
    Dreadbloon,
    Lych, 
    Phayze,
    Blastapopoulus,
    Diamondback
}


public class Main : BloonsTD6Mod
{

    private static bool settingsDirty = false;


    public static readonly ModSettingEnum<BossType> BossName = new(BossType.Bloonarius)
    {
        displayName = "Boss Type",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingBool Elite = new(false)
    {
        displayName = "Elite",
        description = "Check if Boss should be elite or not",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingBool Ranked = new(false)
    {
        displayName = "Ranked",
        description = "Check if Boss should be ranked or not",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingDouble BossHealthMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Health Multiplier",
        description = "Set the Percentage",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingDouble BossSpeedMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Speed Multiplier",
        description = "Set the Percentage",
        onValueChanged = (value) => settingsDirty = true

    };


    public override void OnApplicationStart()
    {
        ModHelper.Msg<Main>("CEditorBoss loaded!");
    }

    public override void OnUpdate()
    {

        if (!settingsDirty) return;
        if (Game.instance?.playerService?.Player == null) return;

        var challengeEditorModel = Game.instance.playerService.Player.Data.challengeEditorModel;

        BossType boss = BossName;
        bool elite = Elite;
        bool ranked = Ranked;


        challengeEditorModel.roundSets.Clear();
        challengeEditorModel.roundSets.Add(BossName?.GetValue()?.ToString()?.ToLower());

        challengeEditorModel.bloonModifiers.healthMultipliers.boss = BossHealthMultiplier / 100;
        challengeEditorModel.bloonModifiers.bossSpeedMultiplier = BossSpeedMultiplier / 100;
        challengeEditorModel.startRules.endRound = 140;

        settingsDirty = false;
     
    }
}