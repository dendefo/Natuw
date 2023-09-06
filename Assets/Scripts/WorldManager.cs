using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    static public WorldManager Instance;
    public Player PlayerReference;
    public int PlayerXP;
    public int PlayerLevel;
    public List<string> LevelScenesNames;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        NextLevel();
    }
    private void Update()
    {
        while (PlayerXP >= CalculateXpForNextLevel())
        {
            LevelUp();
        }
    }

    public void AddXpToPlayer(int XP)
    {
        PlayerXP += XP;
    }
    public int CalculateXpForNextLevel()
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
    public void LevelUp()
    {
        PlayerXP -= CalculateXpForNextLevel();
        PlayerLevel++;
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(LevelScenesNames[0]);
        LevelScenesNames.Remove(LevelScenesNames[0]);
    }
}
