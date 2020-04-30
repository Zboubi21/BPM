using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectFollowTarget : MonoBehaviour
{
    Transform target;
    private void Awake()
    {
        target = PlayerController.s_instance.m_references.m_cameraPivot.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(target);
    }
}
