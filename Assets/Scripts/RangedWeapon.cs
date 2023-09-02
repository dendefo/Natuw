using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class RangedWeapon : MonoBehaviour
{

    [SerializeField] protected SpriteRenderer WeaponSprite;
    [SerializeField] protected GameObject ProjectilePrefab;
    [SerializeField] protected float SpeedMultiplier;
    [SerializeField] protected float DamageMultiplier;

    virtual public Creature ChoseTarget(bool isLookingForEnemy = true)
    {
        Player player = SceneManager.Instance.Player;
        if (isLookingForEnemy)
        {
            if (SceneManager.Instance.EnemyList.Count == 0) return null;

            //God save us from this stupid implementation. This shit is returning the closest enemy. Sometime i'll make it better (and faster)
            return SceneManager.Instance.EnemyList.Find(enemy => enemy.Distance(player) == SceneManager.Instance.EnemyList.Min(enemy => enemy.Distance(player)));

        }
        else return player;

    }
    virtual public void Aim()
    {

    }
    virtual public void Shoot()
    {

    }
    static int ClosestEnemy()
    {
        return -1;
    }
}