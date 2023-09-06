using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{
    #region UnityFunctions
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    protected override void FixedUpdate()
    {
        if (LevelManager.Instance.isPaused && rb.simulated)
        {
            rb.simulated = false;
            return;
        }
        else if(!LevelManager.Instance.isPaused &&!rb.simulated) rb.simulated = true;
        base.FixedUpdate();
        UserInput();
    }
    override protected void Update()
    {
        if (LevelManager.Instance.isPaused&& rb.simulated)
        {
            rb.simulated = false;
            return;
        }
        else if (!LevelManager.Instance.isPaused && !rb.simulated) rb.simulated = true;
        base.Update();
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashReady) Dash();
        PlayAnimation("PlayerSpeed", "PlayerJumpSpeed");

    }
    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(Attributes.DashSpeed * Attributes.DashTime, 0, 0)*(SRenderer.flipX?-1:1)));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, CalculateJumpHeight(), 0));
    }
    #endregion
    private void UserInput()
    {
        if (isInDash) return;
        if (Input.GetKey(KeyCode.A)) Move(false);
        if (Input.GetKey(KeyCode.D)) Move(true);
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) Stop();
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) Stop();
        if (Input.GetKey(KeyCode.Space)) Jump();
    }
}