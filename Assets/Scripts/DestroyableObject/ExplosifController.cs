using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosifController : DestroyableObjectController
{
    
    [SerializeField] int m_newColliderLayerNbr = 16;
    [SerializeField] float m_upForce = 3;
    [SerializeField] Sounds m_impactSounds;

    protected override void On_ObjectIsBreak()
    {
        base.On_ObjectIsBreak();

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.gameObject.layer = m_newColliderLayerNbr;
        }

        StartSoundFromArray(m_impactSounds.m_audioSource, m_impactSounds.m_sounds, m_impactSounds.m_volume, m_impactSounds.m_volumeRandomizer, m_impactSounds.m_pitch, m_impactSounds.m_pitchRandomizer);
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyThirdCategorie);

        Rigidbody rbody;
        rbody = GetComponent<Rigidbody>();
        if (rbody == null)
        rbody = GetComponentInChildren<Rigidbody>();
        rbody.AddForce(rbody.transform.up * m_upForce);
    }

    
}
