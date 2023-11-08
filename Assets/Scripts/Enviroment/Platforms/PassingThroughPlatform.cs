using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingThroughPlatform : MonoBehaviour
{
    public PlatformEffector2D effector;

    public IEnumerator BlockPassing()
    {
        yield return new WaitForSeconds(0.3f);
        PassPlayerDown(false);
    }
    public void PassPlayerDown(bool isDown)
    {
        if (isDown) { effector.colliderMask &= ~(1 << 3); }
        else effector.colliderMask |= (1 << 3);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.S)) { PassPlayerDown(true); StartCoroutine(BlockPassing()); }

#else
        if (WorldManager.Instance.Joystick.Vertical<=-0.707f&& WorldManager.Instance.Joystick.gameObject.active) 
        { 
            if (WorldManager.Instance.Joystick.CenterReference.position.y-WorldManager.Instance.Joystick.StickRect.position.y < 60)
            {
                Debug.Log(WorldManager.Instance.Joystick.CenterReference.position.y-WorldManager.Instance.Joystick.StickRect.position.y);
                return;
            }
            PassPlayerDown(true); 
            StartCoroutine(BlockPassing()); 
        }
#endif
    }
}
