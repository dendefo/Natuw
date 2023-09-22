using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onion : Ground, IEnemy
{
    [SerializeField] private int XpOnDeath = 50;
    [SerializeField] private float knockbackForce;
    public bool canJump;

    private Vector2 LastSeen;
    private bool isMovingRight;

    public int XPOnDeath { get; set; }


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    private void Update()
    {
        LookForPlayer();
        if (Angered && Mathf.Abs((LastSeen - (Vector2)transform.position).x) < 0.5) Angered = false;
        if (Angered) Move(LastSeen.x > transform.position.x);
        else ContinuePatrol();

        PlayAnimation("EnemySpeed");
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
        var player = LevelManager.Instance.Player;
        if (player.transform.position.y - 1 > transform.position.y) { return; }
        var hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, 100, layerMask: LayerMask.GetMask("Player", "TileMap"));
        if (hit.rigidbody == null) {  return; }
        if (hit.rigidbody.CompareTag("Player")) { LastSeen = hit.point; Angered = true; }

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
    #endregion
}
