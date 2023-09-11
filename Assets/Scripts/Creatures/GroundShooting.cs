using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundShooting : Ground
{
    public RangedWeapon weapon;
    [SerializeField] LineRenderer TargetLine;
    private float LastShotTime;
    private Creature Target;

    #region UnityFunctions
    private void OnLevelWasLoaded(int level)
    {
        LastShotTime = 0;
    }
    virtual protected void Update()
    {
        if (weapon == null) return;
        Aim(Target);
        if (LevelManager.Instance.inGameTimer - LastShotTime >= Attributes.AttackSpeed)
        {
            weapon.Shoot(Attributes.CalculateProjectileDamage(), Attributes.BulletFlightSpeed);
            LastShotTime = LevelManager.Instance.inGameTimer;
        }
    }
    override protected void FixedUpdate()
    {
        if (weapon != null)
        {
            Target = weapon.ChoseTarget(LevelManager.Instance.Player == this);
        }
        base.FixedUpdate();
    }
    virtual protected void OnDrawGizmos()
    {
        if (weapon == null) return;
        if (Target == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Target.transform.position, weapon.transform.position);
        Gizmos.DrawWireSphere(Target.transform.position, 1);
    }
    #endregion

    virtual public void Aim(Creature target)
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
        TargetLine.SetPosition(1, Target.transform.position);
    }

}
