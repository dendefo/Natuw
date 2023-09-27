using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBumerang : AbilityScript
{
    [SerializeField] int CollisionAmount;
    [SerializeField] float Speed;
    [SerializeField] SpriteRenderer SR;
    [SerializeField] int _damage;

    private float timeSinceSpawn;
    private float yPosition;
    float playerX;
    private void Start()
    {
        yPosition = WorldManager.Instance.PlayerReference.transform.position.y;
        timeSinceSpawn = WorldManager.Instance._timeWithoutPauses;
        playerX = Camera.main.WorldToScreenPoint(WorldManager.Instance.PlayerReference.transform.position).x / Screen.width / 2;
    }

    private void FixedUpdate()
    {
        //SR.flipX = !SR.flipX;
        float xPosition = Camera.main.ScreenToWorldPoint(new Vector2((Mathf.Sin((1 / Speed) * Mathf.PI * (WorldManager.Instance._timeWithoutPauses - timeSinceSpawn - playerX)) + 1) * (Screen.width / 2), 0)).x;
        transform.position = new(xPosition, yPosition);

        if (WorldManager.Instance._timeWithoutPauses - timeSinceSpawn > (Speed / 2) + CollisionAmount * Speed)
        {
            Destroying();
        }
    }

    private void Destroying()
    {
        float lol = (WorldManager.Instance._timeWithoutPauses - timeSinceSpawn) - ((Speed / 2) + CollisionAmount * Speed);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, lol / 0.5f);
        if (lol > 0.5f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) collision.gameObject.GetComponent<Creature>().GetDamage(_damage);
    }

}
