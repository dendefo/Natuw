using Assets.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelUpgrades : ScriptableObject
{
    public Sprite UpgradeIcon;
    public string UpgradeName;
    public string UpgradeDescription;
    public abstract void Apply(Player player);
}
