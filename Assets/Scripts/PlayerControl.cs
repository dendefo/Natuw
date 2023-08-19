using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float MoveForce;
    [SerializeField] float JumpForce;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool isTouchingFloor;
    void FixedUpdate()
    {
        UserInput();
    }

    private void UserInput()
    {
        if (Input.GetKey(KeyCode.A)) Move(false);
        else if (Input.GetKey(KeyCode.D)) Move(true);

        if (Input.GetKey(KeyCode.Space)&& isTouchingFloor) Jump();
    }
    private void Jump()
    {
        rb.AddForce(Vector3.up* JumpForce*Time.fixedDeltaTime,ForceMode2D.Impulse);
        
    }
    private void Move(bool isRight)
    {
        transform.Translate(MoveForce * Vector3.right * (isRight ? 1 : -1) * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TileMap")
        {
            if (collision.contacts[collision.contactCount-1].point.y<transform.position.y) isTouchingFloor = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TileMap")
        {
            if (collision.contactCount == 0) isTouchingFloor = false;
        }
    }
    
}
