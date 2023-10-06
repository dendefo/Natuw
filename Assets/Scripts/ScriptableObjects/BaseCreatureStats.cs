using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Stats", menuName = "QuarterGames/New Creature")]
public class BaseCreatureStats : ScriptableObject
{
    [Min(0)] public float MaxHp;
    [Min(0)] public float DMG;
    [Min(0)] public float AttackSpeed;
    [Min(0)] public float BulletFlightSpeed;
    [Range(0, 1)] public float CritChance;
    [Min(0)] public float CritDamageMultiplier;
    public int XPOnDeath;
    public int CoinsAmount;

    [Header("Movement")]
    public float MoveVelocity;
    public float JumpVelocity;
}
