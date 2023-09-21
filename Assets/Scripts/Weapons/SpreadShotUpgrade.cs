﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Double Upgrade", menuName = "QuarterGames/Upgrades/SpreadShot Upgrade")]
    internal class SpreadShotUpgrade : WeaponUpgrade
    {
        [SerializeField] float AngleDistance;
        public override void Activate(RangedWeapon weapon, float damage, float speed)
        {
            Vector3 isRight = new(1, 1, 1);
            if ((weapon.transform.rotation * weapon.ProjectileSpawnPoint).x < 0)
            {
                isRight = new(-1, 1, 1);
            }
            Quaternion rotation1 = Quaternion.Euler(0f, 0f, weapon.transform.eulerAngles.z + (AngleDistance / 2));
            Quaternion rotation2 = Quaternion.Euler(0f, 0f, weapon.transform.eulerAngles.z - (AngleDistance / 2));
            Vector3 FirstProjSpawnPoint = Vector3.Scale(rotation1 * weapon.ProjectileSpawnPoint, isRight);
            Vector3 SecondProjSpawnPoint = Vector3.Scale(rotation2 * weapon.ProjectileSpawnPoint, isRight);
            Vector3 WeaponPos = weapon.transform.position;

            var proj = Instantiate(weapon.ProjectilePrefab, WeaponPos + FirstProjSpawnPoint, rotation1);
            var direction = ((WeaponPos + FirstProjSpawnPoint) - WeaponPos).normalized;
            proj.Shoot(direction, speed, damage, weapon.Target != LevelManager.Instance.Player);

            var proj2 = Instantiate(weapon.ProjectilePrefab, WeaponPos + SecondProjSpawnPoint, rotation2);
            var direction2 = ((WeaponPos + SecondProjSpawnPoint) - WeaponPos).normalized;
            proj2.Shoot(direction2, speed, damage, weapon.Target != LevelManager.Instance.Player);
        }
    }
}
