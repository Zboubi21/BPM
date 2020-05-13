using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObjectController : AudioController
{

    [SerializeField] int m_lifePoint = 1;
    [SerializeField] protected UnityEvent m_onObjectIsBreak;

    bool m_isBroken = false;

    protected virtual void Start()
    {
        DestroyableObject[] obj = GetComponentsInChildren<DestroyableObject>();
        for (int i = 0, l = obj.Length; i < l; ++i)
        {
            obj[i].Controller = this;
        }
    }

    public void TakeDamage(int damage)
    {
        if (m_isBroken)
            return;
        
        m_lifePoint -= damage;
        On_ObjectTakeDamage();

        if (m_lifePoint <= 0)
        {
            m_isBroken = true;
            On_ObjectIsBreak();
        }
    }

    protected virtual void On_ObjectTakeDamage()
    {
    }

    protected virtual void On_ObjectIsBreak()
    {
        m_onObjectIsBreak?.Invoke();
    }

}
