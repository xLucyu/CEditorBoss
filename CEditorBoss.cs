using MelonLoader;
using BTD_Mod_Helper;
using CEditorBoss;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Unity;
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
    Blastapopulous
}


public class Main : BloonsTD6Mod
{

    public static BossType selectedBoss {  get; set; }
    public static bool isElite { get; set; }
    public static bool isRanked { get; set; }


    public static ModSettingEnum<BossType> BossName = new(BossType.Bloonarius)
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

    public static ModSettingBool Elite = new(false)
    {
        displayName = "Elite",
        description = "Check if Boss should be elite or not"
    };

    public static ModSettingBool Ranked = new(false)
    {
        displayName = "Ranked",
        description = "Check if Boss should be ranked or not"
    };

    public static ModSettingInt BossHealthMultiplier = new(1)
    {
        displayName = "Boss Health Multiplier",
        min = 0,
        max = 2000
    };

    public static ModSettingInt BossSpeedMultiplier = new(1)
    {
        displayName = "Boss Speed Multiplier",
        min = 0,
        max = 2000
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

            challengeEditorModel.bloonModifiers.healthMultipliers.boss = BossHealthMultiplier;
            challengeEditorModel.bloonModifiers.bossSpeedMultiplier = BossSpeedMultiplier;
        }
    }
}