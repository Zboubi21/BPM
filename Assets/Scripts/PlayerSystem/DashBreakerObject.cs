using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBreakerObject : MonoBehaviour
{
    
    int m_damage = 0;
    public int Damage { set => m_damage = value; }
    bool m_isDashing = false;

    public void On_StartDash(bool dash)
    {
        m_isDashing = dash;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!m_isDashing)
            return;
            
        if (col.CompareTag("DestroyableObject"))
        {
            DestroyableObject destroyableObject = col.GetComponent<DestroyableObject>();
            if (destroyableObject != null)
                destroyableObject.TakeDamage(m_damage);

            DestroyableObjectController destroyableObjectController = col.GetComponent<DestroyableObjectController>();
            if (destroyableObjectController != null)
                destroyableObjectController.TakeDamage(m_damage);
        }
    }
    
}
