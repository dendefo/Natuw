using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(SceneManager.Instance.Player.transform.position.x, SceneManager.Instance.Player.transform.position.y,transform.position.z);
    }
}
