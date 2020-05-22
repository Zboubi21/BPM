using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveDestroyableObject : DestroyableObjectController
{
    
    [SerializeField] ChangeShaderValue[] m_shaders;
    [SerializeField] bool m_hasToFallWhenIsBreak = false;
    [SerializeField] int m_newColliderLayerNbr = 16;
    [SerializeField] Sounds m_impactSounds;
    [SerializeField] bool m_takeDamageJusteOneTime = true;

    Collider[] m_colliders;
    bool m_hasTakeDamage = false;

    protected override void Start()
    {
        base.Start();
        m_colliders = GetComponentsInChildren<Collider>();
    }

    protected override void On_ObjectTakeDamage()
    {
        if (m_hasTakeDamage && m_takeDamageJusteOneTime)
            return;
        m_hasTakeDamage = true;
        base.On_ObjectTakeDamage();
        StartSoundFromArray(m_impactSounds.m_audioSource, m_impactSounds.m_sounds, m_impactSounds.m_volume, m_impactSounds.m_volumeRandomizer, m_impactSounds.m_pitch, m_impactSounds.m_pitchRandomizer);
    }

    protected override void On_ObjectIsBreak()
    {
        base.On_ObjectIsBreak();

        // StartSoundFromArray(m_impactSounds.m_audioSource, m_impactSounds.m_sounds, m_impactSounds.m_volume, m_impactSounds.m_volumeRandomizer, m_impactSounds.m_pitch, m_impactSounds.m_pitchRandomizer);
        
        // On_DeactivateShader();

        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyFirstCategorie);

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
