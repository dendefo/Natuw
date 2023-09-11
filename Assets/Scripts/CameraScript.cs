using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Rect Boundries;
    [SerializeField] Camera cam;
    [Tooltip("How fast does camera follows the player \n 0 - don't, 1 - immediatlly")][Range(0, 1)][SerializeField] float CameraSpeed;
    void Update()
    {
        var x = Mathf.Min(Mathf.Max(LevelManager.Instance.Player.transform.position.x, Boundries.xMin + (cam.orthographicSize * cam.aspect)), Boundries.xMax - (cam.orthographicSize * cam.aspect));
        var y = Mathf.Min(Mathf.Max(LevelManager.Instance.Player.transform.position.y, Boundries.yMin + (cam.orthographicSize)), Boundries.yMax - (cam.orthographicSize));
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), CameraSpeed);
    }
    #region Gizmo
    private Vector3 LeftTopCorner() => new Vector3(Boundries.xMin, Boundries.yMin);
    private Vector3 RightTopCorner() => new Vector3(Boundries.xMax, Boundries.yMin);
    private Vector3 LeftBottomCorner() => new Vector3(Boundries.xMin, Boundries.yMax);
    private Vector3 RightBottomCorner() => new Vector3(Boundries.xMax, Boundries.yMax);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(LeftTopCorner(), RightTopCorner());
        Gizmos.DrawLine(RightTopCorner(), RightBottomCorner());
        Gizmos.DrawLine(RightBottomCorner(), LeftBottomCorner());
        Gizmos.DrawLine(LeftBottomCorner(), LeftTopCorner());
    }
    #endregion
}
