using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public delegate void LevelClearedEventHandler();
    public event LevelClearedEventHandler LevelCleared;

    static public LevelManager Instance;
    public Tilemap TileMap;
    public Player Player;
    public List<IEnemy> EnemyList;
    public PlayerSpawnPoint spawnPoint;

    void Awake()
    {
        if (Player == null) { Player = WorldManager.Instance.PlayerReference; }
        else { WorldManager.Instance.PlayerReference = Player; }
        Player.transform.position = spawnPoint.transform.position;
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        EnemyList = new();
        Instance = this;
    }
    private void Update()
    {
        if (EnemyList.Count == 0) { LevelCleared?.Invoke(); }
    }
    private void OnEnable()
    {
        IEnemy.Death += OnEnemyDeath;
    }

    private void OnEnemyDeath(IEnemy enemy)
    {
        EnemyList.Remove(enemy);
        WorldManager.Instance.AddXpToPlayer(enemy.XPOnDeath);
    }
    private void OnDisable()
    {
        IEnemy.Death -= OnEnemyDeath;
    }
}
