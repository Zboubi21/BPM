using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    
    DestroyableObjectController m_controller;
    public DestroyableObjectController Controller { get => m_controller; set => m_controller = value; }

    public void TakeDamage(int damage)
    {
        m_controller.TakeDamage(damage);
    }
    
}