using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTailladeDeVennes : MonoBehaviour
{
    
    [SerializeField] Transform m_target;

    [SerializeField] float m_followSpeed = 1;

    void LateUpdate()
    {
        transform.rotation = m_target.rotation;

        // transform.position = m_target.position;
        transform.position = Vector3.Lerp(transform.position, m_target.position, Time.deltaTime * m_followSpeed);
    }

}
