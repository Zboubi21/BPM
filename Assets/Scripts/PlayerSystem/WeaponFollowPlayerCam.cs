using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollowPlayerCam : MonoBehaviour
{
   
   [SerializeField] Transform m_target;
   [SerializeField] Vector3 m_offset;
   [SerializeField] float m_speed = 1;

   Vector3 m_targetPos;

    void Start()
    {
        
    }
   void LateUpdate()
   {
        m_targetPos = m_target.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_speed);
   }
   
}
