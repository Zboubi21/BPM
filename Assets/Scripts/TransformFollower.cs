using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    
    [SerializeField] Transform m_followedTrans;
    [SerializeField] UpdateType m_updateMethod = UpdateType.LateUpdate;

    [Header("Position")]
    [SerializeField] bool m_useChangePos = true;
    [SerializeField] Freeze m_freezePos;

    [Tooltip("Use a world value")]
    [SerializeField] Vector3 m_freezePosValue = Vector3.zero;
    [SerializeField] bool m_changeWorldPos = true;

    [Space]
    [Header("Rotation")]
    // [SerializeField] bool m_useRotationAtStart = false;
    Vector3 m_startRotation;
    [SerializeField] bool m_useChangeRot = true;
    [SerializeField] Freeze m_freezeRot;

    [Tooltip("Use a world value")]
    [SerializeField] Vector3 m_freezeRotValue = Vector3.zero;

    [System.Serializable] class Freeze
    {
        public bool m_freezeX = true;
        public bool m_freezeY = true;
        public bool m_freezeZ = true;
    }

    enum UpdateType
    {
        FixedUpdate,
        Update,
        LateUpdate
    }

    Vector3 m_desiredPos;
    Vector3 m_desiredRot;

    void Start()
    {
        // if (m_useRotationAtStart)
            // m_startRotation = transform.eulerAngles;
    }
    public void UpdateScript()
    {
        UpdatePosition();
        UpdateRotation();
    }
    // void FixedUpdate()
    // {
    //     if(m_updateMethod == UpdateType.FixedUpdate)
    //     {
    //         UpdatePosition();
    //         UpdateRotation();
    //     }
    // }
    // void Update()
    // {
    //     if(m_updateMethod == UpdateType.Update)
    //     {
    //         UpdatePosition();
    //         UpdateRotation();
    //     }
    // }
    // void LateUpdate()
    // {
    //     if(m_updateMethod == UpdateType.LateUpdate)
    //     {
    //         UpdatePosition();
    //         UpdateRotation();
    //     }
    // }

    void UpdatePosition()
    {
        if (!m_useChangePos)
            return;
        
        if(m_followedTrans != null)
        {
            if(!m_freezePos.m_freezeX)
            {
                m_desiredPos.x = m_followedTrans.position.x;
            }
            else
            {
                m_desiredPos.x = m_freezePosValue.x;
            }

            if(!m_freezePos.m_freezeY)
            {
                m_desiredPos.y = m_followedTrans.position.y;
            }
            else
            {
                m_desiredPos.y = m_freezePosValue.y;
            }

            if(!m_freezePos.m_freezeZ)
            {
                m_desiredPos.z = m_followedTrans.position.z;
            }
            else
            {
                m_desiredPos.z = m_freezePosValue.z;
            }

            if (m_changeWorldPos)
            {
                transform.position = m_desiredPos;
            }
            else
            {
                transform.localPosition = m_desiredPos;
            }
        }
    }
    void UpdateRotation()
    {
        if (!m_useChangeRot)
            return;

        if(m_followedTrans != null)
        {
            if(!m_freezeRot.m_freezeX)
            {
                m_desiredRot.x = m_followedTrans.rotation.eulerAngles.x;
                // m_desiredRot.x = m_useRotationAtStart ? m_startRotation.x + m_followedTrans.rotation.eulerAngles.x: m_followedTrans.rotation.eulerAngles.x;
            }
            else
            {
                m_desiredRot.x = m_freezeRotValue.x;
            }

            if(!m_freezeRot.m_freezeY)
            {
                m_desiredRot.y = m_followedTrans.rotation.eulerAngles.y;
                // m_desiredRot.y = m_useRotationAtStart ? m_startRotation.y + m_followedTrans.rotation.eulerAngles.y: m_followedTrans.rotation.eulerAngles.y;
            }
            else
            {
                m_desiredRot.y = m_freezeRotValue.y;
            }

            if(!m_freezeRot.m_freezeZ)
            {
                m_desiredRot.z = m_followedTrans.rotation.eulerAngles.z;
                // m_desiredRot.z = m_useRotationAtStart ? m_startRotation.z + m_followedTrans.rotation.eulerAngles.z: m_followedTrans.rotation.eulerAngles.z;
            }
            else
            {
                m_desiredRot.z = m_freezeRotValue.z;
            }

            transform.rotation = Quaternion.Euler(m_desiredRot);
        }
    }
    
}
