using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{

    [SerializeField] TMPro.TMP_Text LevelText;
    [SerializeField] Slider LevelSlider;

    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject DeathCanvas;

    public void UpdateLevel(int PlayerLevel, int PlayerXP, int XpToNextLevel)
    {
        LevelText.text = (PlayerLevel + 1).ToString();
        LevelSlider.value = (float)PlayerXP / XpToNextLevel;
    }

    public void PauseMenuActive(bool pause)
    {
        PauseMenu.SetActive(pause);
    }

    public void DeathMenu()
    {
        DeathCanvas.SetActive(true);
    }

}
