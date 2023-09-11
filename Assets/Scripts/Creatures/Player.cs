using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GroundShooting
{

    #region Fields
    protected bool isDashReady = false;
    protected bool isInDash = false;
    private float DashTimer;
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashTime;
#if !UNITY_EDITOR
    const float JOYSTICK_ERROR_VALUE = 0.05f;
    private bool _isDown = false;
#endif
    #endregion
    #region Movement
    public void Dash()
    {
        DashTimer = Time.time;
        isInDash = true;
        isDashReady = false;

    }

    protected void CalculateJump()
    {
        //if (rb.velocity.y == 0) isTouchingFloor = true;
        //else isTouchingFloor = false;
        
    }
    #endregion
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
        if (IsTouchingFloor && !isInDash) isDashReady = true;
        if (isInDash)
        {
            rb.velocity = (SRenderer.flipX ? -1 : 1) * DashSpeed * Vector2.right + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity);

        }
        if (isInDash && Time.time - DashTimer > DashTime)
        {
            isInDash = false;
            rb.velocity = Vector2.zero;
        }
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
#if !UNITY_EDITOR
        StartCoroutine(UserInput());
#else
       StartCoroutine(UserInput(Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D),
            Input.GetKeyDown(KeyCode.Space),
            Input.GetKey(KeyCode.Space),
            Input.GetKeyUp(KeyCode.Space)
            ));
#endif
    }
#if !UNITY_EDITOR
    public void AndroidJump(bool isDown)
    {
        _isDown = isDown;
        if (isDown) { StartJump(); }
        else StartCoroutine(EndJumpDelayed());
    }
#endif
    public IEnumerator EndJumpDelayed()
    {
        yield return new WaitForSeconds(0.025f);
        EndJump();
    }
    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(DashSpeed * DashTime, 0, 0) * (SRenderer.flipX ? -1 : 1)));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, CalculateJumpHeight(), 0));
    }
#endregion

    private IEnumerator UserInput(bool left = false,bool right = false, bool stop = false, bool startJump = false, bool jump = false, bool endjump = false)
    {
        if (isInDash) yield return null;
        yield return new WaitForFixedUpdate();
#if !UNITY_EDITOR
        Debug.Log(WorldManager.Instance.Joystick.Horizontal);
        if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal < -JOYSTICK_ERROR_VALUE) Move(false);
        else if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal > JOYSTICK_ERROR_VALUE) Move(true);
        else Stop();
#else
        if (left) Move(false);
        if (right) Move(true);
        if (stop) Stop();
        if (startJump) StartJump();
        if (endjump) StartCoroutine(EndJumpDelayed());
#endif

    }
}