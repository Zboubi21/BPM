using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwitchAninm : MonoBehaviour
{
    
    [SerializeField] KeyCode m_debugInput = KeyCode.M;

    Animator m_animator;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(m_debugInput) && m_animator != null)
        {
            m_animator.SetTrigger("NextAnim");
        }
    }
    
}
