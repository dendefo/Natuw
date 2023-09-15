using Assets.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Level Upgrade",menuName = "QuarterGames/Level Upgrade")]

public class LevelUpgrades : ScriptableObject
{
    public bool IsWeaponUpgrade;
    public WeaponUpgrade WeaponUpgrade;
    public Sprite UpgradeIcon;
    public string UpgradeName;
    public string UpgradeDescription;
}
