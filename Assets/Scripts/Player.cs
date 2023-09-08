using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{
    const float JOYSTICK_ERROR_VALUE = 0.05f;
    private bool _isDown = false;
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
        else if (!LevelManager.Instance.isPaused && !rb.simulated) rb.simulated = true;
        base.FixedUpdate();
        UserInput();
    }
    override protected void Update()
    {

        if (LevelManager.Instance.isPaused && rb.simulated)
        {
            rb.simulated = false;
            return;
        }
        else if (!LevelManager.Instance.isPaused && !rb.simulated) rb.simulated = true;
        base.Update();
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashReady) Dash();
        PlayAnimation("PlayerSpeed", "PlayerJumpSpeed");
#if UNITY_ANDROID
        if (_isDown) Jump();
#else
        if (Input.GetKeyDown(KeyCode.Space)) StartJump();
        if (Input.GetKey(KeyCode.Space)) Jump();
        if (Input.GetKeyUp(KeyCode.Space)) StartCoroutine(EndJumpDelayed());
#endif
    }
#if UNITY_ANDROID
    public void AndroidJump(bool isDown)
    {
        _isDown = isDown;
        if (isDown) { StartJump(); }
        else StartCoroutine(EndJumpDelayed());
    }
#endif
    public IEnumerator EndJumpDelayed()
    {
        yield return new WaitForSeconds(0.05f);
        EndJump();
    }
    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(Attributes.DashSpeed * Attributes.DashTime, 0, 0) * (SRenderer.flipX ? -1 : 1)));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, CalculateJumpHeight(), 0));
    }
    #endregion
    private void UserInput()
    {
        if (isInDash) return;
#if UNITY_ANDROID
        Debug.Log(WorldManager.Instance.Joystick.Horizontal);
        if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal < -JOYSTICK_ERROR_VALUE) Move(false);
        else if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal > JOYSTICK_ERROR_VALUE) Move(true);
        else Stop();
#else
        if (Input.GetKey(KeyCode.A)) Move(false);
        if (Input.GetKey(KeyCode.D)) Move(true);
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) Stop();
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) Stop();
#endif

    }
}