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
    [SerializeField] GameObject WinCanvas;

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

    public void WinMenu()
    {
        WinCanvas.SetActive(true);
    }

    public void Feedback()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdMIUtqCNF3MAnuHCE3muLgKqEb_3kR7BdwSWNdw8yEYq7f8Q/viewform?usp=sf_link");
    }
}
