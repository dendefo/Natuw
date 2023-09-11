using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Creature : MonoBehaviour
{
    #region Fields

    [Header("Battle")]
    public CreatureAttributes Attributes;
    public BaseCreatureStats BaseStats;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer SRenderer;
    [SerializeField] protected Animator animator;

    #endregion
    #region UnityFunctions
    private void Awake()
    {
        Attributes = new CreatureAttributes(BaseStats);
    }
    #endregion
    #region BattleFunctions
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

    public CreatureAttributes(BaseCreatureStats creatureStats)
    {
        MaxHP = creatureStats.MaxHp * (1 + (WorldManager.Instance.difficulty.EnemyHP * (WorldManager.Instance.CurrentLevel - 1)));
        HP = MaxHP;
        DMG = creatureStats.DMG * (1 + (WorldManager.Instance.difficulty.EnemyDamage * (WorldManager.Instance.CurrentLevel - 1)));
        AttackSpeed = creatureStats.AttackSpeed;
        BulletFlightSpeed = creatureStats.BulletFlightSpeed * (1 + (WorldManager.Instance.difficulty.BulletSpeed * (WorldManager.Instance.CurrentLevel - 1)));
        CritChance = creatureStats.CritChance;
        CritDamageMultiplier = creatureStats.CritDamageMultiplier;
        MoveVelocity = creatureStats.MoveVelocity * (1 + (WorldManager.Instance.difficulty.EnemySpeed * (WorldManager.Instance.CurrentLevel /5)));
        JumpVelocity = creatureStats.JumpVelocity;
    }
    readonly public float CalculateProjectileDamage()
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