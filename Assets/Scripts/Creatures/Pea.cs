using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pea : Ground, IShooter, IEnemy
{
    [SerializeField] private int XpOnDeath = 50;
    [SerializeField] private float knockbackForce;

    private bool isMovingRight;

    public int XPOnDeath { get; set; }


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    protected override void OnEnable() { base.OnEnable(); (this as IShooter).Enabling(); }
    protected void Update()
    {
        LookForPlayer();
        if (Angered)
        {
            Stop();
            weapon.ChoseTarget(false);
            ((IShooter)this).Aim(weapon.Target, weapon, weapon.TargetLine, transform);
        }
        else ContinuePatrol();

        PlayAnimation("EnemySpeed");

    }
    protected override void OnDisable() { base.OnDisable(); (this as IShooter).Disabling(); }
    override protected void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 force = ((Vector2)LevelManager.Instance.Player.transform.position + collision.collider.offset - ((Vector2)transform.position + collision.otherCollider.offset)).normalized * knockbackForce;
            LevelManager.Instance.Player.GetDamage(Attributes.DMG, force);
        }
        if (collision.gameObject.CompareTag("TileMap"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.x >= 0.707) { isMovingRight = false; Angered = false; }
                else if (contact.normal.x <= -0.707) { isMovingRight = true; Angered = false; }
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        if (collision.gameObject.CompareTag("TileMap"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.x >= 0.707) { isMovingRight = true; Angered = false; }
                else if (contact.normal.x <= -0.707) { isMovingRight = false; Angered = false; }
            }
        }
    }
    #endregion
    #region Movement

    private void LookForPlayer()
    {
        Angered = false;
        var player = LevelManager.Instance.Player;
        var hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, 100, layerMask: LayerMask.GetMask("Player", "TileMap"));
        if (hit.rigidbody == null) { return; }
        if (hit.rigidbody.CompareTag("Player")) { Angered = true; }

    }
    private void ContinuePatrol()
    {
        Move(isMovingRight);
    }

    #endregion

    #region Battle
    protected override void Die()
    {
        ((IEnemy)this).InvokeDeath(this);
        base.Die();
    }

    void IShooter.OnWeaponUpdate(float timeStamp)
    {
        if (((int)(timeStamp * 50)) % ((int)(Attributes.AttackSpeed * 50)) == 0)
        {
            weapon.Shoot(Attributes.DMG, Attributes.BulletFlightSpeed);
        }
    }
    #endregion
}
