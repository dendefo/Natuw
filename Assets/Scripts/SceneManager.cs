using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneManager : MonoBehaviour
{
    static public SceneManager Instance;
    public Tilemap TileMap;
    public Player Player;
    public List<Enemy> EnemyList;
    public bool isPaused;
    public float inGameTimer;

    void Start()
    {
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
