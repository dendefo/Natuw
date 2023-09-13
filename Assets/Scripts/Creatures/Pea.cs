using Assets.Scripts.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pea : Ground, IShooter
{
    [SerializeField] private int XpOnDeath = 50;
    [SerializeField] private float knockbackForce;

    private bool isMovingRight;


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    private void OnEnable() => ((IShooter)this).Enabling();
    protected void Update()
    {
        LookForPlayer();
        if (Angered)
        {
            Stop();
            weapon.ChoseTarget(false);
            ((IShooter)this).Aim(weapon.Target,weapon,weapon.TargetLine,transform);
        }
        else ContinuePatrol();

        PlayAnimation("EnemySpeed");

    }
    private void OnDisable() => ((IShooter)this).Disabling();
    private void OnDestroy()
    {
        WorldManager.Instance.PlayerXP += XpOnDeath;
    }
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
        LevelManager.Instance.EnemyList.Remove(this);
        base.Die();
    }

    void IShooter.OnWeaponUpdate(float timeStamp)
    {
        if (((int)(timeStamp * 50)) % ((int)(Attributes.AttackSpeed * 50)) == 0)
        {
            weapon.Shoot(Attributes.DMG,Attributes.BulletFlightSpeed);
        }
    }
    #endregion
}
