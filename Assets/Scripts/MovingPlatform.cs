using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Rigidbody2D rb;

    [SerializeField] Vector3 leftBorder;
    [SerializeField] Vector3 rightBorder;
    public float speed;
    [SerializeField] bool isMovingRight;

    private void Awake()
    {
        WorldManager.Instance.OnPause+=Pause;
    }
    private void Pause(bool isPaused)
    {
        rb.bodyType = isPaused ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        
    }
    private void FixedUpdate()
    {
        var DirectionLeftToRight = Vector3.Normalize(leftBorder - rightBorder);
        var DirectionRightToLeft = Vector3.Normalize(rightBorder - leftBorder);
        if (isMovingRight) rb.velocity = DirectionRightToLeft * speed;
        else rb.velocity = DirectionLeftToRight * speed;
        if (Vector3.Normalize(transform.position - rightBorder).x > 0 && isMovingRight) isMovingRight = false;
        if (Vector3.Normalize(transform.position - leftBorder).x < 0 && !isMovingRight) isMovingRight = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(leftBorder, rightBorder);
    }
    private void OnDestroy()
    {
        WorldManager.Instance.OnPause -= Pause;
    }
}
