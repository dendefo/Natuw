using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{

    [SerializeField] protected SpriteRenderer WeaponSprite;
    [SerializeField] protected Projectile ProjectilePrefab;
    [SerializeField] protected float SpeedMultiplier;
    [SerializeField] protected float DamageMultiplier;
    [SerializeField] protected float AttackSpeedMultiplier;
    [SerializeField] Transform ProjectileSpawnPoint;
    [SerializeField] Creature Target;

    private void Update()
    {
        if (SceneManager.Instance.inGameTimer % (float)AttackSpeedMultiplier <= 0.02) Shoot();
    }
    virtual public Creature ChoseTarget(bool isLookingForEnemy = true)
    {
        Player player = SceneManager.Instance.Player;
        if (isLookingForEnemy)
        {
            if (SceneManager.Instance.EnemyList.Count == 0) { Target = null; return Target; }

            //God save us from this stupid implementation. This shit is returning the closest enemy. Sometime i'll make it better (and faster)
            Target = SceneManager.Instance.EnemyList.Find(enemy => enemy.Distance(player) == SceneManager.Instance.EnemyList.Min(enemy => enemy.Distance(player)));
            return Target;

        }
        Target = player;
        return player;

    }
    virtual public void Shoot()
    {
        Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, transform.rotation).Shoot(Target.transform.position,SpeedMultiplier,DamageMultiplier);
    }
}