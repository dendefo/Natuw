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
        abstract public void Activate(RangedWeapon weapon, float damage, float speed);

        public override void Apply(Player player)
        {
            player.weapon.AddUpgrade(this);
        }
        protected virtual void Shoot(RangedWeapon weapon, float damage, float speed, Vector3 position, Quaternion rotation, Vector3 direction)
        {
            var proj = Instantiate(weapon.ProjectilePrefab, position, Quaternion.Euler(rotation.eulerAngles + new Vector3(0, 0, 90)));
            proj.Shoot(direction, speed, damage, weapon.Target != LevelManager.Instance.Player);
        }
    }
}
public enum UpgradeType
{
    ForwardShooter,
    SideShooter,
    ForwardMultiplier
}