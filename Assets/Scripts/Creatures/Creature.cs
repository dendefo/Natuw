using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Creature : MonoBehaviour
{
    #region Fields

    [Header("Battle")]
    [SerializeField] public RangedWeapon weapon;
    [SerializeField] public CreatureAttributes Attributes;
    [SerializeField] LineRenderer TargetLine;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer SRenderer;
    [SerializeField] protected Animator animator;

    private float LastShotTime;
    private Creature Target;

    #endregion

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
    virtual protected void FixedUpdate()
    {
        if (weapon != null)
        {
            Target = weapon.ChoseTarget(LevelManager.Instance.Player == this);
        }
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

    #region BattleFunctions
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

    public void GetDamage(float _damage, Vector2 knockback = new())
    {
        Attributes.GetDamage(_damage);
        if (Attributes.HP <= 0) Die();
        rb.velocity += knockback;
        if (knockback != Vector2.zero) Debug.Log(knockback);

    }
    virtual protected void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region VisualAndSound

    protected virtual void PlayAnimation(string speedParameter = null, string jumpParameter = null)
    {
        if (speedParameter != null) animator.SetFloat(speedParameter, Mathf.Abs(rb.velocity.x));
        if (jumpParameter != null) animator.SetFloat(jumpParameter, Mathf.Abs(rb.velocity.y));
    }
    #endregion

    #region Misc
    public float Distance(Creature creature) => Vector3.Distance(transform.position, creature.transform.position);
    #endregion
}

[System.Serializable]
public struct CreatureAttributes
{
    [Min(0)] public float MaxHP;
    [Min(0)] public float HP;
    [Min(0)] public float DMG;
    [Min(0)] public float AttackSpeed;
    [Min(0)] public float BulletFlightSpeed;
    [Range(0, 1)] public float CritChance;
    [Min(0)] public float CritDamageMultiplier;
    [Header("Movement")]
    public float MoveVelocity;
    public float JumpVelocity;
    public float DashTime;
    public float DashSpeed;

    public float CalculateProjectileDamage()
    {
        var rand = Random.Range(0, 1.0f);
        if (rand <= CritChance) return DMG * CritDamageMultiplier;
        else return DMG;
    }
    public void GetDamage(float incomingDamage)
    {
        if (incomingDamage < 0) return;
        HP -= incomingDamage;
        if (HP < 0) HP = 0;
    }

    public void UpgradeMaxHealth()
    {
        MaxHP *= 1.5f;
        HP *= 1.5f;
    }
    public void UpgradeMovementSpeed()
    {
        MoveVelocity *= 1.1f;
    }

    public void AttackSpeedUpgrade()
    {
        AttackSpeed /= 1.25f;
    }
    public void DMGUpgrade()
    {
        DMG *= 1.25f;
    }

}