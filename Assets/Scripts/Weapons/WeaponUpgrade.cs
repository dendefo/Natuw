using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Weapons
{
    [Serializable]
    abstract public class WeaponUpgrade : UnityEngine.ScriptableObject
    {
        public UpgradeType upgradeType;
        abstract public void Activate(RangedWeapon weapon,float damage, float speed);
    }
}
public enum UpgradeType
{
    ForwardShooter,
    SideShooter,
    ForwardMultiplier
}