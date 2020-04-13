using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearObjectController : DestroyableObjectController
{
    
    [SerializeField] float m_waitTimeToDestroy = 0.25f;

    protected override void On_ObjectIsBreak()
    {
        // Faire spawn le FX
        StartCoroutine(WaitTimeToDestroy());
    }
    
    IEnumerator WaitTimeToDestroy()
    {
        yield return new WaitForSeconds(m_waitTimeToDestroy);
        Destroy(gameObject);
    }

}
