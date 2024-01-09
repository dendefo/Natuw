using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public int PlayerXp;
    public int PlayerLevel;
    public int CoinsInRun = 0;

    [SerializeField] List<LevelUpgrades> PossibleLevelUpgrades;
    [SerializeField] List<RewardButton> RewardButtons;
    [SerializeField] GameObject LevelUpCanvas;
    [SerializeField] List<TMPro.TMP_Text> CoinCounters;

    public List<LevelUpgrades> RecievedUpgrades;


    private void Start()
    {
        WorldManager.Instance.hudManager.UpdateLevel(PlayerLevel, PlayerXp, CalculateXpForNextLevel());
    }
    private int CalculateXpForNextLevel()
    {

        switch (PlayerLevel)
        {
            case >= 0 and <= 1:
                return (PlayerLevel + 1) * 50;
            case >= 2 and <= 18:
                return 100 + (PlayerLevel - 1) * 20;
            case >= 19:
                return 440 + (PlayerLevel - 18) * 25;
            case < 0:
                return 0;
        }

    }
    private void LevelUp()
    {
        int nextLevelXP = CalculateXpForNextLevel();
        PlayerXp -= nextLevelXP;
        PlayerLevel++;
        WorldManager.Instance.hudManager.UpdateLevel(PlayerLevel, PlayerXp, nextLevelXP);
        WorldManager.Instance.RewardManager.LevelUpActive(true);
        WorldManager.Instance.LevelUpPausing(true);
    }
    private void LevelUpActive(bool pause)
    {
        LevelUpCanvas.SetActive(pause);
    }
    private void ChooseRewards(bool isPaused)
    {
        bool Predicate(LevelUpgrades upgrade)
        {
            if (upgrade.isInfinite) return true;
            if (RecievedUpgrades.Contains(upgrade)) return false;
            if (upgrade.Dependency is null) return true;
            foreach (var up in RecievedUpgrades)
            {
                if (up.GetInstanceID() == upgrade.Dependency.GetInstanceID()) return true;
            }
            return false;
        }
        var pull = PossibleLevelUpgrades.Where(Predicate).ToList();

        for (int i = 0; i < 3; i++)
        {
            RewardButtons[i].Upgrade = pull[Random.Range(0, pull.Count)];
            pull.Remove(RewardButtons[i].Upgrade);
        }
    }
    private void CheckForLevelUp()
    {
        if (PlayerXp >= CalculateXpForNextLevel() && LevelManager.Instance.EnemyList.Count == 0 && !LevelUpCanvas.active)
        {
            ChooseRewards(true);
            LevelUp();
        }
    }

    public void OnEnemyDeath(IEnemy enemy)
    {
        PlayerXp += enemy.XPOnDeath;
        CoinsInRun += (enemy as Creature).BaseStats.CoinsAmount;
        foreach (var counter in CoinCounters)
        {
            counter.text = CoinsInRun.ToString();
        }
        CheckForLevelUp();
    }
    public void Upgrade(LevelUpgrades upgrade)
    {
        if (!RecievedUpgrades.Contains(upgrade)) RecievedUpgrades.Add(upgrade);
        //List<LevelUpgrades> upgrades = new List<LevelUpgrades>(3);
        //upgrades.Add(upgrade);
        //var a = RewardButtons.Where(x => x.Upgrade != upgrade).ToList();
        //upgrades.Add(a[0].Upgrade);
        //upgrades.Add(a[1].Upgrade);
        //Analytics.PlayerPickedUpgrade(upgrades);
        WorldManager.Instance.RewardManager.LevelUpActive(false);
        WorldManager.Instance.LevelUpPausing(false);

        upgrade.Apply(WorldManager.Instance.PlayerReference);
        CheckForLevelUp();
    }

}
