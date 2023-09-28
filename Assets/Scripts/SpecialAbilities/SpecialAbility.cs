using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAbility : MonoBehaviour
{
    [SerializeField] int OnEnemyHitIncrement;
    [SerializeField] int OnPlayerHitDiscrement;
    [SerializeField] int ReadyPercentage;

    [SerializeField] AbilityScript ability;
    [SerializeField] Image ReadyBar;
    
    public delegate void AbilityReadyEventHandler(SpecialAbility ability,bool isReady);
    public event AbilityReadyEventHandler AbilityReady;
    private void OnEnable()
    {
        Creature.GotDamage += OnCreature_GotDamage;
        Debug.Log("Activated");

    }

    private void OnCreature_GotDamage(Creature creature)
    {
        if (creature is IEnemy) AddPercentage(OnEnemyHitIncrement);
        else if (creature is Player) RemovePercentage(OnPlayerHitDiscrement);
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
        ReadyBar.fillAmount = ReadyPercentage / 100.0f;
    }

    private void RemovePercentage(int amount)
    {
        ReadyPercentage -= amount;
        AbilityReady?.Invoke(this, false);
        if (ReadyPercentage <= 0)
        {
            ReadyPercentage = 0;
        }
        ReadyBar.fillAmount = ReadyPercentage / 100.0f;
    }
    public void Activate()
    {
        Instantiate(ability,WorldManager.Instance.PlayerReference.transform.position,Quaternion.identity);
        ReadyPercentage = 0;
        RemovePercentage(100);
    }
}
