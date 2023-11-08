using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExitPoint : MonoBehaviour
{
    [SerializeField] SpriteRenderer SR;
    [SerializeField] Collider2D Trigger;
    [SerializeField] ParticleSystem particles;
    private void OnEnable()
    {
        LevelManager.Instance.LevelCleared += OnLevelCleared;
    }

    private void OnLevelCleared()
    {
        Trigger.enabled = true;
        particles.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        LevelManager.Instance.LevelCleared -= OnLevelCleared;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") WorldManager.Instance.NextLevel();
    }
}
