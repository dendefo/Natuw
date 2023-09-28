using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Basic Upgrade", menuName = "QuarterGames/Upgrades/Basic Upgrade")]
    internal class OneProjUpgrade : WeaponUpgrade
    {
        public override void Activate(RangedWeapon weapon, float damage, float speed)
        {
            var direction = (weapon.Target.transform.position - (weapon.transform.position + (weapon.transform.rotation * weapon.ProjectileSpawnPoint))).normalized;
            Shoot(weapon,damage,speed, weapon.transform.position + (weapon.transform.rotation * weapon.ProjectileSpawnPoint), weapon.transform.rotation,direction);
        }
    }
}
