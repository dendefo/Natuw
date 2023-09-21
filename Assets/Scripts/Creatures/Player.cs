using Assets.Scripts.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Ground, IShooter
{

    #region Fields
    protected bool isDashReady = false;
    protected bool isInDash = false;
    private float DashTimer;
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashTime;
    [SerializeField] Transform WeaponSolverTarget;
    [SerializeField] Transform WeaponSolver;

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
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ((IShooter)this).Enabling();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsTouchingFloor && !isInDash) isDashReady = true;
        if (isInDash)
        {
            int direction = transform.GetChild(0).eulerAngles.y != 0 ? -1 : 1;
#if !UNITY_EDITOR
            if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal < -JOYSTICK_ERROR_VALUE) direction = -1;
            else if (WorldManager.Instance.Joystick.gameObject.active && WorldManager.Instance.Joystick.Horizontal > JOYSTICK_ERROR_VALUE) direction = 1;
#else
            if (Input.GetKey(KeyCode.D)) direction = 1;
            else if (Input.GetKey(KeyCode.A)) direction = -1;
#endif
            rb.velocity = (direction * DashSpeed * Vector2.right + (_currentConnectedPlatform == null ? Vector2.zero : _currentConnectedPlatform.rb.velocity));

        }
        if (isInDash && Time.time - DashTimer > DashTime)
        {
            isInDash = false;
            rb.velocity = Vector2.zero;
        }
    }
    protected void Update()
    {
        weapon.ChoseTarget();

        if (rb.bodyType == RigidbodyType2D.Static) return;
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
    protected override void OnDisable()
    {
        base.OnDisable();
        ((IShooter)this).Disabling();
    }
    protected override void Die()
    {
        Analytics.PlayerDied();
        base.Die();
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
    protected void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(DashSpeed * DashTime, 0, 0) * (SRenderer.flipX ? -1 : 1)));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, CalculateJumpHeight(), 0));
    }
    #endregion

    private IEnumerator UserInput(bool left = false, bool right = false, bool stop = false, bool startJump = false, bool jump = false, bool endjump = false)
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

        ((IShooter)this).Aim(weapon.Target, weapon, weapon.TargetLine, transform);

    }

    void IShooter.OnWeaponUpdate(float timeStamp)
    {
        if (((int)(timeStamp * 50)) % ((int)(Attributes.AttackSpeed * 50)) == 0)
        {
            weapon.Shoot(Attributes.DMG, Attributes.BulletFlightSpeed);
        }
    }

    void IShooter.Aim(Creature target, RangedWeapon weapon, LineRenderer TargetLine, Transform transform)
    {
        if (target != null)
        {
            if (target.transform.position.x < WeaponSolver.position.x) transform.GetChild(0).eulerAngles = new Vector3(0, 180, 0);
            else transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
        }
        if (target == null) WeaponSolverTarget.localPosition = new Vector3(4.5f, 1, 0);
        else WeaponSolverTarget.position = target.transform.position;

        if (TargetLine == null && target == null) return;
        else if (TargetLine != null && target == null)
        {
            TargetLine.SetPosition(0, WeaponSolver.position);
            TargetLine.SetPosition(1, WeaponSolver.position);
            return;
        }
        TargetLine.SetPosition(0, WeaponSolver.position);
        TargetLine.SetPosition(1, target.transform.position);
    }

}