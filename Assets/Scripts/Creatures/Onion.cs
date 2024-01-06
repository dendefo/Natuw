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

    private bool isMovingRight;

    public int XPOnDeath { get; set; }


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    private void Update()
    {

        Move(isMovingRight);
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
                if (contact.normal.x >= 0.707) isMovingRight = false;
                else if (contact.normal.x <= -0.707) isMovingRight = true;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (transform.position.x + 1 - collision.collider.bounds.max.x > 0) isMovingRight = false;
            if (transform.position.x - 1 - collision.collider.bounds.min.x < 0) isMovingRight = true;
        }
        if (collision.gameObject.CompareTag("TileMap"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.x >= 0.707) isMovingRight = true;
                else if (contact.normal.x <= -0.707) isMovingRight = false;
            }
            if (!LevelManager.Instance.TileMap.HasTile(LevelManager.Instance.TileMap.WorldToCell(new(transform.position.x + (isMovingRight ? 1 : -1), transform.position.y - 1, transform.position.z)))) isMovingRight = !isMovingRight;
        }
        base.OnCollisionStay2D(collision);

    }
    #endregion
    #region Movement

    #endregion

    #region Battle
    protected override void Die()
    {
        ((IEnemy)this).InvokeDeath(this);
        base.Die();
        
    }

   
    #endregion
}
