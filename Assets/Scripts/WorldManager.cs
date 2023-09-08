using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        NextLevel();
    }
    private void Update()
    {
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
        while (PlayerXP >= CalculateXpForNextLevel())
        {
            LevelUp();
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
}
