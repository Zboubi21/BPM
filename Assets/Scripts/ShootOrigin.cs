using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOrigin : MonoBehaviour
{
    
    [SerializeField] Transform m_targetTrans;
    [SerializeField] float m_yLocalFreezePos;

    void FixedUpdate()
    {
        transform.position = m_targetTrans.position;
        transform.localPosition = new Vector3(transform.localPosition.x, m_yLocalFreezePos, transform.localPosition.z);
    }
    
}
