using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats Upgrade", menuName = "QuarterGames/Stats Upgrade")]
public class StatsUpgrade : LevelUpgrades
{
    public StatType UpgradeType;
    public float Multiplier;


    public override void Apply(Player player)
    {
        switch (UpgradeType)
        {
            case StatType.MaxHealth:
                player.Attributes.UpgradeMaxHealth(Multiplier);
                break;
            case StatType.AttackSpeed:
                player.Attributes.AttackSpeedUpgrade(Multiplier);
                break;
            case StatType.Damage:
                player.Attributes.DMGUpgrade(Multiplier);
                break;
            case StatType.MovementSpeed:
                player.Attributes.UpgradeMovementSpeed(Multiplier);
                break;
        }
    }
}
public enum StatType
{
    MaxHealth,
    AttackSpeed,
    Damage,
    MovementSpeed
}
