using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BreakingPart : AudioController
{
    
    [SerializeField] float m_minTriggerSoundMagnitude = 10;
    [SerializeField] float m_timeToDoSoundAgain = 1f;
    [SerializeField] Sounds m_impactSound;

    Rigidbody m_rbody;
    AudioSource m_audioSource;
    bool m_canDoSound = true;

    void Start()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        // Debug.Log("relativeVelocity = " + col.relativeVelocity.magnitude);
        if (!m_canDoSound)
            return;

        if (col.gameObject.CompareTag("Player"))
            return;

        if (col.relativeVelocity.magnitude > m_minTriggerSoundMagnitude)
        {
            if (m_impactSound.m_audioSource != null)
                StartSoundFromArray(m_impactSound.m_audioSource, m_impactSound.m_sounds, m_impactSound.m_volume, m_impactSound.m_volumeRandomizer, m_impactSound.m_pitch, m_impactSound.m_pitchRandomizer);
            else
                if (m_audioSource != null)
                    StartSoundFromArray(m_audioSource, m_impactSound.m_sounds, m_impactSound.m_volume, m_impactSound.m_volumeRandomizer, m_impactSound.m_pitch, m_impactSound.m_pitchRandomizer);
        }
    }

    IEnumerator WaitToDoSound()
    {
        m_canDoSound = false;
        yield return new WaitForSeconds(m_timeToDoSoundAgain);
        m_canDoSound = true;
    }

}
