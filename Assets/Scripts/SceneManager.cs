using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneManager : MonoBehaviour
{
    public Tilemap TileMap;
    public Player Player;
    static public SceneManager Instance;
    public List<Enemy> EnemyList;

    void Start()
    {
        EnemyList = new();
        Instance = this;
    }
}
