using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    public int PlayerXP;
    public int PlayerLevel;
    public List<string> LevelScenesNames;
    public bl_Joystick Joystick;
    public TMPro.TMP_Text FPSCounter;
    [SerializeField] TMPro.TMP_InputField DebuggerInput;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject LevelUpCanvas;

    bool _isPaused;
    float _timeWithoutPauses;

    void Start()
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
        if (PlayerXP >= CalculateXpForNextLevel() && LevelManager.Instance.EnemyList.Count == 0 && !LevelUpCanvas.active)
        {
            LevelUpPausing(true);
            LevelUp();
        }
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

    public void AddXpToPlayer(int XP)
    {
        PlayerXP += XP;
    }
    public int CalculateXpForNextLevel()
    {

        switch (PlayerLevel)
        {
            case >= 0 and <= 1:
                return (PlayerLevel + 1) * 50;
            case >= 2 and <= 18:
                return 100 + (PlayerLevel - 1) * 20;
            case >= 19:
                return 440 + (PlayerLevel - 18) * 25;
            case < 0:
                return 0;
        }

    }
    public void LevelUp()
    {
        PlayerXP -= CalculateXpForNextLevel();
        PlayerLevel++;
    }

    public void NextLevel()
    {
        if (LevelScenesNames.Count == CurrentLevel) SceneManager.LoadSceneAsync("MainMenu");
        else SceneManager.LoadSceneAsync(LevelScenesNames[CurrentLevel++]);
    }
    public void Debugger()
    {
        switch (DebuggerInput.text.ToLower())
        {
            case "upgrade_health":
                Upgrade((int)UpgradeTypes.MaxHealth);
                DebuggerInput.text = "";
                break;
            case "upgrade_double_jump":
                Upgrade((int)UpgradeTypes.DoubleJump);
                DebuggerInput.text = "";
                break;
            case "upgrade_attack_speed":
                Upgrade((int)UpgradeTypes.FireRate);
                DebuggerInput.text = "";
                break;
            case "upgrade_damage":
                Upgrade((int)UpgradeTypes.Damage);
                DebuggerInput.text = "";
                break;
            case "upgrade_movement":
                Upgrade((int)UpgradeTypes.MovementSpeed);
                DebuggerInput.text = "";
                break;
            case "upgrade_double_bullets":
                Upgrade((int)UpgradeTypes.DoubleBullets);
                DebuggerInput.text = "";
                break;
            default:
                Debug.Log("Not a command");
                break;
        }
    }
    public void Pausing(bool isPaused)
    {
        OnPause(isPaused);
        _isPaused = isPaused;
        PauseMenu.SetActive(isPaused);
    }
    public void LevelUpPausing(bool isPaused)
    {
        OnPause(isPaused);
        _isPaused = isPaused;
        LevelUpCanvas.SetActive(isPaused);
    }
    public void Upgrade(int num)
    {
        UpgradeTypes upgradeType = (UpgradeTypes)num;
        LevelUpPausing(false);
        switch (upgradeType)
        {
            case (UpgradeTypes.MaxHealth):
                PlayerReference.Attributes.UpgradeMaxHealth();
                break;
            case UpgradeTypes.DoubleBullets:
                PlayerReference.weapon.UpgradeDoubleBullets();
                break;
            case UpgradeTypes.FireRate:
                PlayerReference.Attributes.AttackSpeedUpgrade();
                break;
            case UpgradeTypes.Damage:
                PlayerReference.Attributes.DMGUpgrade();
                break;
            case UpgradeTypes.MovementSpeed:
                PlayerReference.Attributes.UpgradeMovementSpeed();
                break;
            case UpgradeTypes.DoubleJump:
                PlayerReference.UpgradeDoubleJump();
                break;
        }
    }
    private void OnDestroy()
    {
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


public enum UpgradeTypes
{
    MaxHealth,
    DoubleBullets,
    FireRate,
    Damage,
    MovementSpeed,
    DoubleJump
}