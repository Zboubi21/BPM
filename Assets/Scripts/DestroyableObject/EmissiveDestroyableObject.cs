using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveDestroyableObject : DestroyableObjectController
{
    
    [SerializeField] ChangeShaderValue[] m_shaders;
    [SerializeField] bool m_hasToFallWhenIsBreak = false;

    protected override void On_ObjectIsBreak()
    {
        // base.On_ObjectIsBreak();

        On_DeactivateShader();

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
        Rigidbody rbody = GetComponentInChildren<Rigidbody>();
        if (rbody != null)
            rbody.isKinematic = false;
    }

}
