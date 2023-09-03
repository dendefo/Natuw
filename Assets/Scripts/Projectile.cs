using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    private Vector3 _velocity;
    private float _damage;
    private bool _isShootedByPlayer;

    private void FixedUpdate()
    {
        rb.velocity = _velocity;
    }
    public void Shoot(Vector3 position, float speed, float damage,bool isShootedByPlayer = true)
    {
        _isShootedByPlayer = isShootedByPlayer;
        _damage = damage;
        _velocity = (Vector2)(Vector3.Normalize(position - transform.position) * speed);
    }
    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && _isShootedByPlayer) return;
        else if (collision.collider.tag == "Enemy" && _isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.collider.tag == "Player" && !_isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.collider.tag == "Enemy" && !_isShootedByPlayer) return;
        else Destroy(gameObject);
    }
    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _isShootedByPlayer) return;
        else if (collision.tag == "Enemy" && _isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.tag == "Player" && !_isShootedByPlayer) { collision.gameObject.GetComponent<Creature>().GetDamage(_damage); Destroy(gameObject); }
        else if (collision.tag == "Enemy" && !_isShootedByPlayer) return;
        else Destroy(gameObject);
    }
}