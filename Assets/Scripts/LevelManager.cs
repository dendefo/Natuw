using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    static public LevelManager Instance;
    public Tilemap TileMap;
    public Player Player;
    public List<Enemy> EnemyList;
    public bool isPaused;
    public float inGameTimer;
    public PlayerSpawnPoint spawnPoint;
    public GameObject LevelUpCanvas;
    public int PlayerLevelAtStart;

    void Awake()
    {
        if (Player == null) { Player = WorldManager.Instance.PlayerReference; }
        else { WorldManager.Instance.PlayerReference = Player; }
        Player.transform.position = spawnPoint.transform.position;
        if (Instance!=null)
        {
            Destroy(Instance.gameObject);
        }
        EnemyList = new();
        Instance = this;
        PlayerLevelAtStart = WorldManager.Instance.PlayerLevel;
    }
    private void Update()
    {
        if (isPaused)
        {
            return;
        }
        inGameTimer += Time.deltaTime;
        if (EnemyList.Count == 0 && WorldManager.Instance.PlayerLevel!= PlayerLevelAtStart) 
        {
            isPaused = true;
            LevelUpCanvas.SetActive(true);
        }
    }

    public void Upgrade(int num)
    {
        PlayerLevelAtStart++;
        UpgradeTypes upgradeType = (UpgradeTypes)num;

        LevelUpCanvas.SetActive(false);
        isPaused = false;
        switch (upgradeType)
        {
            case (UpgradeTypes.MaxHealth):
                Player.Attributes.UpgradeMaxHealth();
                break;
            case UpgradeTypes.DoubleBullets:
                Player.weapon.UpgradeDoubleBullets();
                break;
            case UpgradeTypes.FireRate:
                Player.Attributes.AttackSpeedUpgrade();
                break;
            case UpgradeTypes.Damage:
                Player.Attributes.DMGUpgrade();
                break;
            case UpgradeTypes.MovementSpeed:
                Player.Attributes.UpgradeMovementSpeed();
                break;
            case UpgradeTypes.DoubleJump:
                break;
        }
    }
}
public enum UpgradeTypes{
    MaxHealth,
    DoubleBullets,
    FireRate,
    Damage,
    MovementSpeed,
    DoubleJump
}