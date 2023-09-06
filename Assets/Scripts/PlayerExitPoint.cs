using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExitPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && LevelManager.Instance.EnemyList.Count == 0) WorldManager.Instance.NextLevel();
    }
}
