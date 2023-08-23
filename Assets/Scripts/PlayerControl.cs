using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float MoveForce;
    [SerializeField] float JumpForce;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool isTouchingFloor;
    TilemapCollider2D tile;
    void FixedUpdate()
    {
        CalculateJump();
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
    private void CalculateJump()
    {
        if (rb.velocity.y == 0) isTouchingFloor = true;
        else isTouchingFloor = false;
    }
    private void Move(bool isRight)
    {
        transform.Translate(MoveForce * Vector3.right * (isRight ? 1 : -1) * Time.fixedDeltaTime);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "TileMap")
    //    {
    //        int legitContacts = 0;
    //        foreach (var contact in collision.contacts)
    //        {
    //            //Debug.Log(((Vector2)transform.position - contact.point).y);
    //            if (((Vector2)transform.position - contact.point).y >= transform.localScale.y) legitContacts++;
    //        }
    //        Debug.Log(legitContacts);
    //        if (legitContacts == 0) isTouchingFloor = true;
    //        else isTouchingFloor = false;

    //    }
    //}
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    OnCollisionEnter2D(collision);

    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "TileMap")
    //    {
    //        //if (collision.contactCount == 0) isTouchingFloor = false;
    //    }
    //}
    
}
