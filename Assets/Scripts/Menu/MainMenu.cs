using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject LastUpdateWindow;

    public void Awake()
    {
        if (PlayerPrefs.HasKey("LastVersion"))
        {
            if (PlayerPrefs.GetString("LastVersion") == Application.version) return;
        }
        PlayerPrefs.SetString("LastVersion", Application.version);
        LastUpdateWindow.SetActive(true);
    }
    public void PlayGame()
    {
        //SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
