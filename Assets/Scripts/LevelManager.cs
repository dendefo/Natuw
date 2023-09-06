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

    void Start()
    {
        if (Instance!=null)
        {
            Destroy(Instance.gameObject);
        }
        EnemyList = new();
        Instance = this;
    }
    private void Update()
    {
        if (isPaused)
        {
            return;
        }
        inGameTimer += Time.deltaTime;
        
    }
}
