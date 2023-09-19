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
            var proj = GameObject.Instantiate(weapon.ProjectilePrefab, weapon.transform.position + (weapon.transform.rotation*weapon.ProjectileSpawnPoint), weapon.transform.rotation);
            var direction = (weapon.Target.transform.position - (weapon.transform.position + (weapon.transform.rotation * weapon.ProjectileSpawnPoint))).normalized;
            proj.Shoot(direction, speed, damage, weapon.Target != LevelManager.Instance.Player);

        }
    }
}
