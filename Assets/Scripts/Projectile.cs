using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    Vector3 velocity;

    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }
    public void Shoot(Vector3 position, float speed, float damage)
    {
        velocity = (Vector2)(Vector3.Normalize((position - transform.position)) * speed);
    }
    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player") return;
        else Destroy(gameObject);
    }
    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") return;
        else Destroy(gameObject);
    }
}
