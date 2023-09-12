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
    public Dictionary<IShooter, float> shooters;
    bool _isPaused;
    float _timeWithoutPauses;

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
        shooters = new()
        {
            { Player, 0 }
        };
        Instance = this;
    }
    private void FixedUpdate()
    {
        if (_isPaused) return;
        _timeWithoutPauses += Time.fixedDeltaTime;
        List<IShooter> shooted = new();
        foreach (var shooter in shooters)
        {

            Creature creature = ((Creature)shooter.Key);
            if (!(creature.Angered || creature == Player)) continue;
            creature.weapon.ChoseTarget(creature == Player);
            shooter.Key.Aim(creature.weapon.Target, creature.weapon, creature.weapon.TargetLine, creature.transform);
            if (_timeWithoutPauses - creature.Attributes.AttackSpeed > shooter.Value)
            {
                creature.weapon.Shoot(creature.Attributes.DMG, creature.Attributes.BulletFlightSpeed);
                shooted.Add(shooter.Key);
            }
        }
        foreach (var shooter in shooted)
        {
            shooters[shooter] = _timeWithoutPauses;
        }
    }
    private void Pausing(bool isPaused)
    {
        _isPaused = isPaused;
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