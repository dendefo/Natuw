using UnityEngine;
using Unity;
using Unity.VisualScripting;

namespace Assets.Scripts.Creatures
{
    public interface IShooter
    {
        protected static void OnEnable(IShooter shooter)
        {
            WorldManager.Instance.WeaponUpdate += shooter.OnWeaponUpdate;
        }
        virtual public void Aim(Creature target,RangedWeapon weapon, LineRenderer TargetLine, Transform transform)
        {
            
            if (target == null)
            {
                weapon.transform.rotation = Quaternion.identity;
                weapon.WeaponSprite.flipY = false;
                if (TargetLine == null) return;

                TargetLine.SetPosition(0, transform.position);
                TargetLine.SetPosition(1, TargetLine.GetPosition(0));

                return;
            }
            void Rotate(Transform toRotate, Vector3 toMove)
            {
                var norm = Vector3.Normalize(toRotate.position - toMove);
                var Acos = Mathf.Acos(norm.y);
                var z = Acos / Mathf.PI * (toRotate.position.x > toMove.x ? -180 : 180);

                toRotate.localEulerAngles = new Vector3(0, 0, z - 90);
                weapon.WeaponSprite.flipY = Mathf.Abs(z - 90) > 90;
            }

            Rotate(weapon.transform, target.transform.position);
            if (TargetLine == null) return;
            TargetLine.SetPosition(0, transform.position);
            TargetLine.SetPosition(1, target.transform.position);
        }

        abstract protected void OnWeaponUpdate(float timeStamp);
        protected static void OnDisable(IShooter shooter)
        {
            WorldManager.Instance.WeaponUpdate -= shooter.OnWeaponUpdate;
        }
    }
}
