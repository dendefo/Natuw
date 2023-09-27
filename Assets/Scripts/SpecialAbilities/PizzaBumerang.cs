using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBumerang : AbilityScript
{
    [SerializeField] int CollisionAmount;
    [SerializeField] float Speed;
    [SerializeField] SpriteRenderer SR;

    private float timeSinceSpawn;
    private float yPosition;
    private void Start()
    {
        yPosition = WorldManager.Instance.PlayerReference.transform.position.y;
        timeSinceSpawn = WorldManager.Instance._timeWithoutPauses;
    }

    private void FixedUpdate()
    {
        //SR.flipX = !SR.flipX;
        float xPosition = Camera.main.ScreenToWorldPoint(new Vector2((Mathf.Sin((1 / Speed) * Mathf.PI * WorldManager.Instance._timeWithoutPauses - timeSinceSpawn) + 1) * (Screen.width / 2), 0)).x;
        transform.position = new(xPosition, yPosition);

        if (WorldManager.Instance._timeWithoutPauses - timeSinceSpawn > (Speed / 2) + CollisionAmount * Speed)
        {
            Destroying();
        }
    }

    private void Destroying()
    {
        float lol = (WorldManager.Instance._timeWithoutPauses - timeSinceSpawn) - ((Speed / 2) + CollisionAmount * Speed);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, lol/0.5f);
        if (lol > 0.5f) Destroy(gameObject);
    }

}
