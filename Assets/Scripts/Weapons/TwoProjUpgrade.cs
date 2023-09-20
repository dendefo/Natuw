﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Double Upgrade", menuName = "QuarterGames/Upgrades/Double Upgrade")]
    internal class TwoProjUpgrade : WeaponUpgrade
    {
        [SerializeField] float AngleDistance;
        public override void Activate(RangedWeapon weapon, float damage, float speed)
        {
            Quaternion rotation1 = Quaternion.Euler(0f, 0f, weapon.transform.eulerAngles.z + (AngleDistance / 2));
            Quaternion rotation2 = Quaternion.Euler(0f, 0f, weapon.transform.eulerAngles.z - (AngleDistance / 2));
            Vector3 FirstProjSpawnPoint = (rotation1 * weapon.ProjectileSpawnPoint);
            Vector3 SecondProjSpawnPoint = (rotation2 * weapon.ProjectileSpawnPoint);
            Vector3 WeaponPos = weapon.transform.position;
            Vector3 TargetPos = weapon.Target.transform.position;

            var proj = Instantiate(weapon.ProjectilePrefab, WeaponPos + FirstProjSpawnPoint, rotation1);
            var direction = (TargetPos + FirstProjSpawnPoint - (WeaponPos + FirstProjSpawnPoint)).normalized;
            proj.Shoot(direction, speed, damage, weapon.Target != LevelManager.Instance.Player);

            var proj2 = Instantiate(weapon.ProjectilePrefab, WeaponPos + SecondProjSpawnPoint, rotation2);
            var direction2 = (TargetPos + SecondProjSpawnPoint - (WeaponPos + SecondProjSpawnPoint)).normalized;
            proj2.Shoot(direction2, speed, damage, weapon.Target != LevelManager.Instance.Player);
        }
    }
}