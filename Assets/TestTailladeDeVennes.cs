using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTailladeDeVennes : MonoBehaviour
{
    
    [SerializeField] Transform m_target;

    void LateUpdate()
    {
        transform.position = m_target.position;
        transform.rotation = m_target.rotation;
    }

}
