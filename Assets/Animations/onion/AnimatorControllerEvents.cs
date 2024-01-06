using Assets.Scripts.Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerEvents : MonoBehaviour
{
    public delegate void ShootAfterAnim();
    public event ShootAfterAnim OnShoot;

    public void DieAfterAnimation()
    {
        Destroy(transform.parent.gameObject);
    }
   public void ShootInAnimation()
    {
        OnShoot.Invoke();
    }
}
