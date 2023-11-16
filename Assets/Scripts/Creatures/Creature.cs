using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

abstract public class Creature : MonoBehaviour, IPausable
{
    #region Fields

    [Header("Battle")]
    public CreatureAttributes Attributes;
    public BaseCreatureStats BaseStats;
    public RangedWeapon weapon;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer SRenderer;
    [SerializeField] private Color DamageColor = new Color(231 / 255.0f, 89 / 255.0f, 89 / 255.0f);
    [SerializeField] public Animator animator;
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] protected Image HpBar;
    public bool Angered;
    public bool IsVulnerable = true;

    public delegate void GotDamageEventHandler(Creature creature);
    public static event GotDamageEventHandler GotDamage;

    #endregion
    #region UnityFunctions
    virtual protected void Awake()
    {
        Attributes = new CreatureAttributes(BaseStats);
        if (this is IEnemy)
        {
            (this as IEnemy).XPOnDeath = BaseStats.XPOnDeath;
        }

        sprites = GetComponentsInChildren<SpriteRenderer>();

    }
    virtual protected void OnEnable()
    {
        WorldManager.Instance.OnPause += Pausing;
    }
    virtual protected void OnDisable()
    {
        WorldManager.Instance.OnPause -= Pausing;
    }

    public void Pausing(bool isPaused)
    {
        rb.bodyType = isPaused ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        animator.enabled = !isPaused;
    }

    #endregion
    #region BattleFunctions
    virtual public void GetDamage(float _damage, Vector2 knockback = new())
    {
        if (!IsVulnerable) return;
        Attributes.GetDamage(_damage);
        HpBar.fillAmount = Attributes.HP / Attributes.MaxHP;
        GotDamage?.Invoke(this);
        if (Attributes.HP <= 0) Die();
        rb.AddForce(knockback, ForceMode2D.Impulse);
        //rb.velocity += knockback;
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.DOColor(DamageColor, 1f).From().ChangeEndValue(Color.white);
        }
    }
    virtual protected void Die()
    {
        animator.SetTrigger("Die");
        rb.bodyType = RigidbodyType2D.Static;
        List<Collider2D> results = new();
        rb.GetAttachedColliders(results);

        foreach (Collider2D collider in results)
        {
            collider.enabled = false;
        }
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
        MoveVelocity = creatureStats.MoveVelocity * (1 + (WorldManager.Instance.difficulty.EnemySpeed * (WorldManager.Instance.CurrentLevel / 5)));
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
    public void UpgradeMaxHealth(float multi)
    {
        MaxHP *= multi;
        HP *= multi;
    }
    public void UpgradeMovementSpeed(float multi)
    {
        MoveVelocity *= multi;
    }
    public void AttackSpeedUpgrade(float multi)
    {
        AttackSpeed /= multi;
        int divRest = (Mathf.RoundToInt(AttackSpeed * 100f)) % 4;
        if (divRest != 0) AttackSpeed = (Mathf.RoundToInt(AttackSpeed * 100f) - divRest) / 100f;
        else AttackSpeed = Mathf.RoundToInt(AttackSpeed * 100f) / 100f;
    }
    public void DMGUpgrade(float multi)
    {
        DMG *= multi;
    }
}