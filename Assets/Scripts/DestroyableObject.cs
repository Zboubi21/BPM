using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    
    bool m_isBroken = false;

    public void BreakObject()
    {
        if (m_isBroken)
            return;

        m_isBroken = true;
        On_ObjectIsBreak();
    }

    void On_ObjectIsBreak()
    {
        Destroy(gameObject);
    }
    
}
