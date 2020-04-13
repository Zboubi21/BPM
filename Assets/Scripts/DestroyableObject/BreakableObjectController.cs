using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjectController : DestroyableObjectController
{

    [SerializeField] GameObject m_baseMesh;
    [SerializeField] GameObject m_breakMesh;
    [SerializeField] float m_breakForce = 3;
    
    protected override void On_ObjectIsBreak()
    {
        m_baseMesh.SetActive(false);
        m_breakMesh.SetActive(true);
        Rigidbody[] rbody = GetComponentsInChildren<Rigidbody>();
        for (int i = 0, l = rbody.Length; i < l; ++i)
        {
            rbody[i].AddForce(rbody[i].transform.position + (-rbody[i].transform.up * m_breakForce));
        }
    }

}
