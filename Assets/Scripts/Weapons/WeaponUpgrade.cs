using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [Serializable]
    abstract public class WeaponUpgrade : LevelUpgrades
    {
        public UpgradeType upgradeType;
        abstract public void Activate(RangedWeapon weapon,float damage, float speed);

        public override void Apply(Player player)
        {
            player.weapon.Upgrades.Add(this);
        }
    }
}
public enum UpgradeType
{
    ForwardShooter,
    SideShooter,
    ForwardMultiplier
}