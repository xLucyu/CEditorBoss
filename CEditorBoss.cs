using MelonLoader;
using BTD_Mod_Helper;
using CEditorBoss;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Unity;
using UnityEngine;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Data.Behaviors.Towers;
using Il2CppAssets.Scripts.Models;
using BTD_Mod_Helper.Extensions;

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
    Blastapopulous
}


public class Main : BloonsTD6Mod
{

    public static BossType selectedBoss {  get; set; }
    public static bool isElite { get; set; }
    public static bool isRanked { get; set; }


    public static ModSettingEnum<BossType> BossName = new ModSettingEnum<BossType>(BossType.Bloonarius)
    {
        displayName = "Boss Type",

        labelFunction = boss => boss switch
        {
            BossType.Bloonarius => "Bloonarius",
            BossType.Vortex => "Vortex",
            BossType.Dreadbloon => "Dreadbloon",
            BossType.Lych => "Lych",
            BossType.Phayze => "Phayze",
            BossType.Blastapopulous => "Blastapopulous",
            _ => boss.ToString()
        }
    };

    public static ModSettingBool Elite = new ModSettingBool(false)
    {
        displayName = "Elite",
        description = "Check if Boss should be elite or not"
    };

    public static ModSettingBool Ranked = new ModSettingBool(false)
    {
        displayName = "Ranked",
        description = "Check if Boss should be ranked or not"
    };

    public static ModSettingDouble BossHealthMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Health Multiplier",
        description = "Set the Percentage",
    };

    public static ModSettingDouble BossSpeedMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Speed Multiplier",
        description = "Set the Percentage",

    };


    public override void OnApplicationStart()
    {
        ModHelper.Msg<Main>("CEditorBoss loaded!");
    }

    public override void OnUpdate()
    {
        if (Game.instance?.playerService?.Player == null) return;

        var challengeEditorModel = Game.instance.playerService.Player.Data.challengeEditorModel;

        if (Input.GetKeyDown(KeyCode.X))
        {
            selectedBoss = BossName;
            isElite = Elite;
            isRanked = Ranked;

            challengeEditorModel.roundSets.Clear();
            challengeEditorModel.roundSets.Add(BossName?.GetValue()?.ToString()?.ToLower()); // credits to @doruat

            challengeEditorModel.bloonModifiers.healthMultipliers.boss = BossHealthMultiplier / 100;
            challengeEditorModel.bloonModifiers.bossSpeedMultiplier = BossSpeedMultiplier / 100;
            challengeEditorModel.startRules.endRound = 140;


        }
    }    
}
