using System;
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

            var direction = ((WeaponPos + FirstProjSpawnPoint) - WeaponPos).normalized;
            var direction2 = ((WeaponPos + SecondProjSpawnPoint) - WeaponPos).normalized;
            
            Shoot(weapon, damage, speed, WeaponPos + FirstProjSpawnPoint, rotation1, direction);
            Shoot(weapon, damage, speed, WeaponPos + SecondProjSpawnPoint, rotation2, direction2);
        }
    }
}
