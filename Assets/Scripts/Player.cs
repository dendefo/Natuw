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
        if (Input.GetKey(KeyCode.A)) Move(false);
        if (Input.GetKey(KeyCode.D)) Move(true);
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) Stop();
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) Stop();

        if (Input.GetKey(KeyCode.Space)) Jump();
    }
}
