using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionDaddy : Onion
{
    [SerializeField] Onion OnionSpawnReference;
    protected override void Die()
    {
        Instantiate(OnionSpawnReference,transform.position+(Vector3.left/2),Quaternion.identity);
        Instantiate(OnionSpawnReference, transform.position + (Vector3.right / 2), Quaternion.identity);
        base.Die();
    }
}
