using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    static public WorldManager Instance;
    public Player PlayerReference;
    public int PlayerXP;
    public int PlayerLevel;
    public List<string> LevelScenesNames;
    public bl_Joystick Joystick;
    public TMPro.TMP_Text FPSCounter;
    [SerializeField] TMPro.TMP_InputField DebuggerInput;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        NextLevel();
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
        while (PlayerXP >= CalculateXpForNextLevel())
        {
            LevelUp();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (Time.timeScale == 0) { Time.timeScale = 1; DebuggerInput.gameObject.SetActive(false); }
            else { Time.timeScale = 0; DebuggerInput.gameObject.SetActive(true); }
        }

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
        SceneManager.LoadSceneAsync(LevelScenesNames[0]);
        LevelScenesNames.Remove(LevelScenesNames[0]);
    }
    public void Debugger()
    {
        switch (DebuggerInput.text.ToLower())
        {
            case "upgrade_health":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.MaxHealth);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            case "upgrade_double_jump":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.DoubleJump);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            case "upgrade_attack_speed":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.FireRate);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            case "upgrade_damage":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.Damage);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            case "upgrade_movement":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.MovementSpeed);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            case "upgrade_double_bullets":
                LevelManager.Instance.Upgrade((int)UpgradeTypes.DoubleBullets);
                DebuggerInput.text = "";
                LevelManager.Instance.PlayerLevelAtStart--;
                break;
            default:
                Debug.Log("Not a command");
                break;
        }
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
