using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class WorldManager : MonoBehaviour
{
    public delegate void Pause(bool isPaused);
    public event Pause OnPause;

    public delegate void WeaponUpdateHandler(float time);
    public event WeaponUpdateHandler WeaponUpdate;

    public DifficultyMultiplyers difficulty;
    public int CurrentLevel;
    static public WorldManager Instance;
    public Player PlayerReference;

    public List<string> LevelScenesNames;
    public bl_Joystick Joystick;

    public HudManager hudManager;
    public RewardSystem RewardManager;

    public TMPro.TMP_Text FPSCounter;

    [SerializeField] TMPro.TMP_InputField DebuggerInput;
    [SerializeField] GameObject DeathCanvas;

    bool _isPaused;
    public float _timeWithoutPauses;

    void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
        DontDestroyOnLoad(Instance);
        NextLevel();
        if (PlayerReference == null) PlayerReference = Instantiate(Resources.Load<Player>("Player"));
    }
    private void Update()
    {
        FPSCounter.text = ((int)(1 / Time.deltaTime)).ToString();
#if !UNITY_EDITOR
        foreach (var touch in Input.touches)
        {
            if (touch.position.x > Screen.width / 2) continue;

            var data = new PointerEventData(EventSystem.current);
            data.pointerId = touch.fingerId;

            if (touch.phase == TouchPhase.Began)
            {
                Joystick.gameObject.SetActive(true);
                Joystick.transform.position = touch.position;
                Joystick.OnPointerDown(data);
            }
            else if (touch.phase == TouchPhase.Moved) Joystick.OnDrag(data);
            else if (touch.phase == TouchPhase.Ended||touch.phase == TouchPhase.Canceled) Joystick.OnPointerUp(data);
        }
#endif
        if (Input.GetKeyDown(KeyCode.F1))
        {

            if (Time.timeScale == 0) { Time.timeScale = 1; DebuggerInput.gameObject.SetActive(false); }
            else { Time.timeScale = 0; DebuggerInput.gameObject.SetActive(true); }
        }

    }
    private void FixedUpdate()
    {
        if (_isPaused) return;
        _timeWithoutPauses += Time.fixedDeltaTime;
        if (((int)(_timeWithoutPauses * 50)) % ((int)(0.04 * 50)) == 0) WeaponUpdate?.Invoke(_timeWithoutPauses);
    }

    public void NextLevel()
    {
        if (LevelScenesNames.Count == CurrentLevel)
        {
            OnPause?.Invoke(true);
            Analytics.PlayerFinishedRun();
            hudManager.WinMenu();
        }
        else SceneManager.LoadSceneAsync(LevelScenesNames[CurrentLevel++]);
    }

    //public void Debugger()
    //{
    //    switch (DebuggerInput.text.ToLower())
    //    {
    //        case "upgrade_health":
    //            Upgrade((int)UpgradeTypes.MaxHealth);
    //            DebuggerInput.text = "";
    //            break;
    //        case "upgrade_double_jump":
    //            Upgrade((int)UpgradeTypes.DoubleJump);
    //            DebuggerInput.text = "";
    //            break;
    //        case "upgrade_attack_speed":
    //            Upgrade((int)UpgradeTypes.FireRate);
    //            DebuggerInput.text = "";
    //            break;
    //        case "upgrade_damage":
    //            Upgrade((int)UpgradeTypes.Damage);
    //            DebuggerInput.text = "";
    //            break;
    //        case "upgrade_movement":
    //            Upgrade((int)UpgradeTypes.MovementSpeed);
    //            DebuggerInput.text = "";
    //            break;
    //        case "upgrade_double_bullets":
    //            Upgrade((int)UpgradeTypes.DoubleBullets);
    //            DebuggerInput.text = "";
    //            break;
    //        default:
    //            Debug.Log("Not a command");
    //            break;
    //    }
    //}
    #region Pauses
    public void Pausing(bool isPaused)
    {
        OnPause(isPaused);
        _isPaused = isPaused;
        hudManager.PauseMenuActive(isPaused);
    }
    public void LevelUpPausing(bool isPaused)
    {
        OnPause(isPaused);
        _isPaused = isPaused;
    }
    public void DeathPausing()
    {
        OnPause(true);
        _isPaused = true;
        hudManager.DeathMenu();
        PlayerReference.animator.enabled = true;
    }
    #endregion

    private void OnDestroy()
    {
        if (PlayerReference.gameObject == null) return;
        Destroy(PlayerReference.gameObject);
    }

#if !UNITY_EDITOR
    public void JumpButton(bool isDown)
    {
        LevelManager.Instance.Player.AndroidJump(isDown);
    }
    public void DashButton()
    {
        LevelManager.Instance.Player.Dash();
    }
#endif
}
[System.Serializable]
public struct DifficultyMultiplyers
{
    public float EnemyHP;
    public float EnemyDamage;
    public float EnemySpeed;
    public float BulletSpeed;
}