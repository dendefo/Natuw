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
    [SerializeField] Transform ProjectileSecondSpawnPoint;
    [SerializeField] Transform ProjectileThirdSpawnPoint;
    public SpriteRenderer WeaponSprite;
    //[SerializeField] ParticleSystem particleProjectile;
    private Creature Target;
    public bool isDoubleShooter = false;
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
        if (Target == null) return;
        if (isDoubleShooter) { Instantiate(ProjectilePrefab, ProjectileSecondSpawnPoint.position, transform.rotation).Shoot((Target.transform.position - ProjectileSpawnPoint.transform.position).normalized, speed * SpeedMultiplier, damage * DamageMultiplier, Target != LevelManager.Instance.Player); }
        Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, transform.rotation).Shoot((Target.transform.position - ProjectileSpawnPoint.transform.position).normalized, speed * SpeedMultiplier, damage * DamageMultiplier, Target != LevelManager.Instance.Player);
       
    }
    public struct WeaponSaveData
    {
        public float Speed;
        public float Damage;
        public float Attack;
        public Projectile prefab;
    }
    public WeaponSaveData GetSaveData()
    {
        var data = new WeaponSaveData();
        data.Speed = SpeedMultiplier;
        data.Damage = DamageMultiplier;
        data.Attack = AttackSpeedMultiplier;
        data.prefab = ProjectilePrefab;
        return data;
    }
    public void LoadSaveData(WeaponSaveData data)
    {
        SpeedMultiplier = data.Speed;
        DamageMultiplier = data.Damage;
        AttackSpeedMultiplier = data.Attack;
        ProjectilePrefab = data.prefab;
    }

    #region Upgrades

    public void UpgradeDoubleBullets()
    {
        isDoubleShooter = true;
        ProjectileSecondSpawnPoint = Instantiate(ProjectileSpawnPoint, this.transform);
        ProjectileSpawnPoint.Translate(Vector3.Scale(ProjectilePrefab.transform.localScale, Vector3.up), Space.Self);
        ProjectileSecondSpawnPoint.Translate(Vector3.Scale(ProjectilePrefab.transform.localScale, Vector3.down), Space.Self);
    }
    #endregion
}