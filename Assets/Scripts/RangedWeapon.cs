using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    #region Fields
    [Header("Multipliers")]
    [SerializeField] protected float SpeedMultiplier;
    [SerializeField] protected float DamageMultiplier;
    [SerializeField] protected float AttackSpeedMultiplier;
    [Header("Visuals")]
    [SerializeField] protected Projectile ProjectilePrefab;
    [SerializeField] Transform ProjectileSpawnPoint;
    public SpriteRenderer WeaponSprite;
    //[SerializeField] ParticleSystem particleProjectile;
    private Creature Target;
    #endregion

    virtual public Creature ChoseTarget(bool isLookingForEnemy = true)
    {
        Player player = LevelManager.Instance.Player;
        if (isLookingForEnemy)
        {
            if (LevelManager.Instance.EnemyList.Count == 0) { Target = null; return Target; }

            //God save us from this stupid implementation. This shit is returning the closest enemy. Sometime i'll make it better (and faster)
            Target = LevelManager.Instance.EnemyList.Find(enemy => enemy.Distance(player) == LevelManager.Instance.EnemyList.Min(enemy => enemy.Distance(player)));

            return Target;

        }
        Target = player;
        return player;

    }
    virtual public void Shoot(float damage, float speed)
    {
        if (Target==null) return;
        Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, transform.rotation).Shoot(Target.transform.position,speed*SpeedMultiplier,damage*DamageMultiplier,Target!= LevelManager.Instance.Player);
    }
}