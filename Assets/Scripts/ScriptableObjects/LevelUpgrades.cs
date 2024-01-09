using Assets.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelUpgrades : ScriptableObject
{
    public Sprite UpgradeIcon;
    public string UpgradeName;
    public string UpgradeDescription;
    public bool isInfinite;
    public LevelUpgrades Dependency;
    public abstract void Apply(Player player);

    static public bool operator ==(LevelUpgrades o1, LevelUpgrades o2)
    {
        if (o1 is null && o2 is null) return true;
        if (o1 is null || o2 is null) return false;
        return (o1.name == o2.name &&
            o1.UpgradeIcon == o2.UpgradeIcon &&
            o1.UpgradeName == o2.UpgradeName &&
            o1.UpgradeDescription == o2.UpgradeDescription &&
            o1.isInfinite == o2.isInfinite &&
            o1.Dependency == o2.Dependency);
    }
    static public bool operator !=(LevelUpgrades o1, LevelUpgrades o2)
    {
        return o1 == o2;
    }
}
