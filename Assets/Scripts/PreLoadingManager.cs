using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Analytics.Start();
        Application.targetFrameRate = 100;
        SceneManager.LoadSceneAsync(1);
    }
}
