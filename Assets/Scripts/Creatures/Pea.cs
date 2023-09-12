using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pea : GroundShooting
{
    [SerializeField] private int XpOnDeath = 50;
    [SerializeField] private float knockbackForce;

    private bool isAfterPlayer;
    private bool isMovingRight;


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    override protected void Update()
    {
        LookForPlayer();
        if (isAfterPlayer) base.Update();
        else ContinuePatrol();

        PlayAnimation("EnemySpeed");
    }
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
                if (contact.normal.x >= 0.707) { isMovingRight = false; isAfterPlayer = false; }
                else if (contact.normal.x <= -0.707) { isMovingRight = true; isAfterPlayer = false; }
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
                if (contact.normal.x >= 0.707) { isMovingRight = true; isAfterPlayer = false; }
                else if (contact.normal.x <= -0.707) { isMovingRight = false; isAfterPlayer = false; }
            }
        }
    }
    #endregion
    #region Movement

    private void LookForPlayer()
    {
        var player = LevelManager.Instance.Player;
        var hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, 100, layerMask: LayerMask.GetMask("Player", "TileMap"));
        if (hit.rigidbody == null) { return; }
        if (hit.rigidbody.CompareTag("Player")) { isAfterPlayer = true; }
        else isAfterPlayer = false;

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
    #endregion
}
