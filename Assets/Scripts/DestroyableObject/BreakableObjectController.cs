using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjectController : DestroyableObjectController
{

    [SerializeField] GameObject m_baseMesh;
    [SerializeField] GameObject m_breakMesh;
    [SerializeField] float m_breakForce = 3;
    [SerializeField] float m_upForce = 3;
    
    protected override void On_ObjectIsBreak()
    {
        m_baseMesh.SetActive(false);
        m_breakMesh.SetActive(true);
        Rigidbody[] rbody = GetComponentsInChildren<Rigidbody>();
        for (int i = 0, l = rbody.Length; i < l; ++i)
        {
            // rbody[i].AddForce(rbody[i].transform.position + (-rbody[i].transform.up * m_breakForce));
            // rbody[i].AddForce(rbody[i].transform.position + (rbody[i].transform.up * m_breakForce));

            Vector3 force = ((rbody[i].transform.position - transform.position).normalized * m_breakForce) + (rbody[i].transform.forward * m_upForce);
            rbody[i].AddForce(force);

            // int alea = Random.Range(0, 100);
            // if (alea <= 50)
            //     rbody[i].AddForce(rbody[i].transform.position + (transform.forward * m_breakForce));
            // if (alea > 50)
            //     rbody[i].AddForce(rbody[i].transform.position + (-transform.forward * m_breakForce));
        }
    }

}
