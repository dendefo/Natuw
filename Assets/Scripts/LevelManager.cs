using Assets.Scripts.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{

    static public LevelManager Instance;
    public Tilemap TileMap;
    public Player Player;
    public List<Creature> EnemyList;
    public PlayerSpawnPoint spawnPoint;

    void Awake()
    {
        WorldManager.Instance.OnPause += Pausing;
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
    
    private void Pausing(bool isPaused)
    {

    }
}

public enum UpgradeTypes
{
    MaxHealth,
    DoubleBullets,
    FireRate,
    Damage,
    MovementSpeed,
    DoubleJump
}