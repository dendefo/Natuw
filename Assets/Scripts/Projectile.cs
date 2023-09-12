using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPausable
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private Vector2 _velocity;
    private float _damage;
    private bool _isShootedByPlayer;

    private void Awake()
    {
        WorldManager.Instance.OnPause += Pausing;
    }
    public void Pausing(bool isPaused)
    {
        rb.bodyType = isPaused ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;

    }

    private void FixedUpdate()
    {
        rb.velocity = _velocity;
    }
    public void Shoot(Vector3 direction, float speed, float damage, bool isShootedByPlayer = true)
    {
        _isShootedByPlayer = isShootedByPlayer;
        _damage = damage;
        _velocity = (Vector2)(direction * speed);
    }
    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && _isShootedByPlayer) return;
        else if (collision.collider.CompareTag("Enemy") && _isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.collider.CompareTag("Player") && !_isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.collider.CompareTag("Enemy") && !_isShootedByPlayer) return;
        else Destroy(gameObject);
    }
    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isShootedByPlayer) return;
        else if (collision.CompareTag("Enemy") && _isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.CompareTag("Player") && !_isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.CompareTag("Enemy") && !_isShootedByPlayer) return;
        else Destroy(gameObject);
    }
    private void OnDestroy()
    {
        WorldManager.Instance.OnPause -= Pausing;
    }
}
