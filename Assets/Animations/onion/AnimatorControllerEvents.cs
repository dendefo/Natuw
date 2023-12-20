using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerEvents : MonoBehaviour
{
    public void DieAfterAnimation()
    {
        Destroy(transform.parent.gameObject);
    }
   
}
