using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveDestroyableObject : DestroyableObjectController
{
    
    [SerializeField] ChangeShaderValue[] m_shaders;
    [SerializeField] bool m_hasToFallWhenIsBreak = false;
    [SerializeField] int m_newColliderLayerNbr = 16;

    Collider[] m_colliders;

    protected override void Start()
    {
        base.Start();
        m_colliders = GetComponentsInChildren<Collider>();
    }

    protected override void On_ObjectIsBreak()
    {
        // base.On_ObjectIsBreak(impactPos);

        On_DeactivateShader();

        //GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.firstCategorie);

        if (m_hasToFallWhenIsBreak)
            On_ObjectFall();
    }

    void On_DeactivateShader()
    {
        if (m_shaders == null)
            return;

        for (int i = 0, l = m_shaders.Length; i < l; ++i)
        {
            m_shaders[i]?.SwitchValue();
        }
    }

    void On_ObjectFall()
    {
        if (m_colliders != null)
        {
            for (int i = 0, l = m_colliders.Length; i < l; ++i)
            {
                m_colliders[i].gameObject.layer = m_newColliderLayerNbr;
            }
        }

        Rigidbody rbody = GetComponentInChildren<Rigidbody>();
        if (rbody != null)
            rbody.isKinematic = false;
    }

}
