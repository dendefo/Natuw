using System;
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
        public override void Activate(RangedWeapon weapon, float damage, float speed)
        {
            throw new NotImplementedException();
        }
    }
}
