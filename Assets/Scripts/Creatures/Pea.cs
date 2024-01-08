using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pea : Ground, IShooter, IEnemy
{
    [SerializeField] AnimatorControllerEvents animatorControllerEvents;
    [SerializeField] private int XpOnDeath = 50;
    [SerializeField] private float knockbackForce;
    [SerializeField] private Transform WeaponSolver;
    [SerializeField] private Transform WeaponSolverTarget;
    private bool isMovingRight;

    public int XPOnDeath { get; set; }


    #region UnityFunctions
    private void Start()
    {
        LevelManager.Instance.EnemyList.Add(this);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        (this as IShooter).Enabling();
        animatorControllerEvents.OnShoot += AnimatorControllerEvents_OnShoot;
    }

    protected void Update()
    {
        LookForPlayer();
        if (Angered)
        {
            Stop();
            PlayAnimation("EnemySpeed");
            weapon.ChoseTarget(false);
            ((IShooter)this).Aim(weapon.Target, weapon, weapon.TargetLine, transform);
        }
        else
        {
            PlayAnimation("EnemySpeed");
            ContinuePatrol();
        }

    }
    protected override void OnDisable()
    {
        base.OnDisable();
        (this as IShooter).Disabling();
        animatorControllerEvents.OnShoot -= AnimatorControllerEvents_OnShoot;
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
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (transform.position.x + 0.25f - collision.collider.bounds.max.x > 0) isMovingRight = false;
            if (transform.position.x - 0.25f - collision.collider.bounds.min.x < 0) isMovingRight = true;
        }
        if (collision.gameObject.CompareTag("TileMap"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.x >= 0.707) { isMovingRight = true; Angered = false; }
                else if (contact.normal.x <= -0.707) { isMovingRight = false; Angered = false; }
            }
            if (!LevelManager.Instance.TileMap.HasTile(LevelManager.Instance.TileMap.WorldToCell(new(transform.position.x + (isMovingRight ? 0.25f : -0.25f), transform.position.y - 1, transform.position.z))))
            {
                List<RaycastHit2D> res = new();
                var circleHits = Physics2D.CircleCast(new(transform.position.x + (isMovingRight ? 0.25f : -0.25f), transform.position.y), 0.5f, Vector2.zero, new ContactFilter2D(), res);
                if (!res.Any(hit => hit.collider.CompareTag("Platform"))) isMovingRight = !isMovingRight;
            }
        }
        base.OnCollisionStay2D(collision);
    }
    #endregion
    #region Movement

    private void LookForPlayer()
    {
        Angered = false;
        var player = LevelManager.Instance.Player;
        var hit = Physics2D.Raycast(TargetPoint.transform.position, (player.TargetPoint.transform.position - TargetPoint.transform.position).normalized, 100, layerMask: LayerMask.GetMask("Player", "TileMap"));
        if (hit.rigidbody == null) { return; }
        if (hit.rigidbody.CompareTag("Player")) { Angered = true; }

    }
    private void ContinuePatrol()
    {
        Move(isMovingRight, true);
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
        if (!Angered) return;
        if (((int)(timeStamp * 50)) % ((int)(Attributes.AttackSpeed * 50)) == 0)
        {
            animator.SetTrigger("Shoot");
        }
    }


    private void AnimatorControllerEvents_OnShoot()
    {
        weapon.Shoot(Attributes.DMG, Attributes.BulletFlightSpeed);
    }
    void IShooter.Aim(Creature target, RangedWeapon weapon, LineRenderer TargetLine, Transform transform)
    {
        if (target != null)
        {
            if (target.transform.position.x < WeaponSolver.position.x) transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
            else transform.GetChild(0).eulerAngles = new Vector3(0, 180, 0);
        }
        if (target == null) WeaponSolverTarget.localPosition = new Vector3(4.5f, 1, 0);
        else WeaponSolverTarget.position = target.transform.position;

        if (TargetLine == null && target == null) return;
        else if (TargetLine != null && target == null)
        {
            TargetLine.SetPosition(0, WeaponSolver.position);
            TargetLine.SetPosition(1, WeaponSolver.position);
            return;
        }
        TargetLine.SetPosition(0, WeaponSolver.position);
        TargetLine.SetPosition(1, target.transform.position);
    }
    #endregion
}
