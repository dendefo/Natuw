using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(LevelManager.Instance.Player.transform.position.x, LevelManager.Instance.Player.transform.position.y,transform.position.z);
    }
}
