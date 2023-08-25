using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UserInput();
    }
    private void UserInput()
    {
        if (isInDash) return;
        if (Input.GetKey(KeyCode.A)) Move(false);
        if (Input.GetKey(KeyCode.D)) Move(true);
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) Stop();
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) Stop();

        if (Input.GetKey(KeyCode.Space)) Jump();
        if(Input.GetKeyDown(KeyCode.LeftShift)&&isDashReady) Dash();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(DashSpeed * DashTime, 0, 0)*(SRenderer.flipX?-1:1)));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, CalculateJumpHeight(), 0));
    }
}
