using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class SpecialAbility : MonoBehaviour
{
    [SerializeField] private int ReadyPercentage;
    
    public delegate void AbilityReadyEventHandler(SpecialAbility ability,bool isReady);
    public event AbilityReadyEventHandler AbilityReady;
    private void OnEnable()
    {
        Creature.GotDamage += OnCreature_GotDamage;
    }

    private void OnCreature_GotDamage(Creature creature)
    {
        if (creature is IEnemy) AddPercentage(5);
        else if (creature is Player) RemovePercentage(5);
    }

    private void OnDisable()
    {
        Creature.GotDamage -= OnCreature_GotDamage;
    }

    private void AddPercentage(int amount)
    {
        ReadyPercentage += amount;
        if (ReadyPercentage >= 100)
        {
            ReadyPercentage = 100;
            AbilityReady?.Invoke(this, true);
        }
    }

    private void RemovePercentage(int amount)
    {
        ReadyPercentage -= amount;
        if (ReadyPercentage <= 0)
        {
            ReadyPercentage = 0;
            AbilityReady?.Invoke(this, false);
        }
    }
    public void Activate()
    {

    }
}
