using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IPausable
{
    public Rigidbody2D rb;

    [SerializeField] Vector3 leftBorder;
    [SerializeField] Vector3 rightBorder;
    public float speed;
    [SerializeField] bool isMovingRight;

    private void Awake()
    {
        WorldManager.Instance.OnPause += Pausing;
    }
    public void Pausing(bool isPaused)
    {
        rb.bodyType = isPaused ? RigidbodyType2D.Static : RigidbodyType2D.Kinematic;

    }
    private void FixedUpdate()
    {
        var DirectionLeftToRight = Vector3.Normalize(leftBorder - rightBorder);
        var DirectionRightToLeft = Vector3.Normalize(rightBorder - leftBorder);
        if (isMovingRight) rb.velocity = DirectionRightToLeft * speed;
        else rb.velocity = DirectionLeftToRight * speed;
        if (Vector2.Distance(transform.position, rightBorder) < 1 && isMovingRight) isMovingRight = false;
        if (Vector2.Distance(transform.position, leftBorder) < 1 && !isMovingRight) isMovingRight = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftBorder, rightBorder);
    }
    private void OnDestroy()
    {
        WorldManager.Instance.OnPause -= Pausing;
    }
}
