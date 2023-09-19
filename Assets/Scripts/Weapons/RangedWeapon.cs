using Assets.Scripts.Weapons;
using System;
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
    public Projectile ProjectilePrefab;
    public Vector3 ProjectileSpawnPoint;
    public List<WeaponUpgrade> Upgrades =  new();

    [SerializeField] Transform ProjectileSecondSpawnPoint;
    [SerializeField] Transform ProjectileThirdSpawnPoint;
    public SpriteRenderer WeaponSprite;
    //[SerializeField] ParticleSystem particleProjectile;
    public Creature Target;
    public LineRenderer TargetLine;
    public bool isDoubleShooter = false;
    #endregion

    virtual public Creature ChoseTarget(bool isLookingForEnemy = true)
    {
        if (LevelManager.Instance == null) return null;
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
        if (Target == null) return;
        foreach (WeaponUpgrade upgrade in Upgrades)
        {
            upgrade.Activate(this, damage * DamageMultiplier, speed * SpeedMultiplier);
        }
    }

    #region Upgrades
    public void AddUpgrade(WeaponUpgrade upgrade)
    {
        if (upgrade == null) return;
        if (upgrade.upgradeType == UpgradeType.ForwardShooter)
        {
            Upgrades.RemoveAll(x => x.upgradeType == UpgradeType.ForwardShooter);
        }
        Upgrades.Add(upgrade);
    }
    #endregion
}