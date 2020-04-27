using System;
using System.Collections;
using UnityEngine;

public class SuicidalEnemyAudioController : AudioController
{

    [SerializeField] MovementSoundLooper m_movements;
    [SerializeField] Sounds m_activateSelfDestruction;

    [SerializeField] SelfDestructionBIP m_selfDestructionBIP;
    [Serializable] class SelfDestructionBIP
    {
        [Header("Timers")]
        public float m_startDelay = 1;
        public float m_minDelay = 0.25f;
        [Range(0, 15)] public uint m_operations = 5;

        [Header("Sounds")]
        public AudioSource m_audioSource;
        public Sound m_sound;
    }

    [SerializeField] Sounds m_explode;
    [SerializeField] Sounds m_die;
    [SerializeField] Sounds m_disintegrate;

    IEnumerator m_waitToExplodeCorout;
    float m_deltaSpeed = 0;
    int m_deltaTick = 0;

    void Start()
    {
        m_deltaSpeed = (m_selfDestructionBIP.m_startDelay - m_selfDestructionBIP.m_minDelay) / (m_selfDestructionBIP.m_operations - 1);
    }

    public void On_StartToMoveFast(bool moveFast)
    {
        if (moveFast)
            m_movements.SwitchPitch();
        else
            m_movements.SetStartPitch();
    }
    public void On_StartToMove(bool isMoving)
    {
        m_movements.On_StartLoop(isMoving);
    }

    public void On_SelfDestructionIsActivated()
    {
        StartSoundFromArray(m_activateSelfDestruction.m_audioSource, m_activateSelfDestruction.m_sounds, m_activateSelfDestruction.m_volume, m_activateSelfDestruction.m_volumeRandomizer, m_activateSelfDestruction.m_pitch, m_activateSelfDestruction.m_pitchRandomizer);
    }

    public void On_EnemyWaitToExplode(float timeToExplode)
    {
        m_deltaTick = 0;
        StartWaitToExplodeCoroutine(true);
    }
    void StartWaitToExplodeCoroutine(bool start)
    {
        if (m_waitToExplodeCorout != null)
            StopCoroutine(m_waitToExplodeCorout);
        if (start)
        {
            m_waitToExplodeCorout = WaitToExplode();
            StartCoroutine(m_waitToExplodeCorout);
        }
    }
    IEnumerator WaitToExplode()
    {
        StartSound(m_selfDestructionBIP.m_audioSource, m_selfDestructionBIP.m_sound.m_sound, m_selfDestructionBIP.m_sound.m_volume, m_selfDestructionBIP.m_sound.m_pitch);
        
        float timeToWait;
        if (m_deltaTick < m_selfDestructionBIP.m_operations)
            timeToWait = m_selfDestructionBIP.m_startDelay - m_deltaSpeed * m_deltaTick;
        else
            timeToWait = m_selfDestructionBIP.m_minDelay;

        m_deltaTick ++;
        yield return new WaitForSeconds(timeToWait);
        StartWaitToExplodeCoroutine(true);
    }

    public void On_EnemyExplode()
    {
        StartWaitToExplodeCoroutine(false);
        StartSoundFromArray(m_explode.m_audioSource, m_explode.m_sounds, m_explode.m_volume, m_explode.m_volumeRandomizer, m_explode.m_pitch, m_explode.m_pitchRandomizer);
    }

    public void On_EnemyDie()
    {
        StartSoundFromArray(m_die.m_audioSource, m_die.m_sounds, m_die.m_volume, m_die.m_volumeRandomizer, m_die.m_pitch, m_die.m_pitchRandomizer);
    }
    public void On_EnemyIsDisintegrate()
    {
        StartSoundFromArray(m_disintegrate.m_audioSource, m_disintegrate.m_sounds, m_disintegrate.m_volume, m_disintegrate.m_volumeRandomizer, m_disintegrate.m_pitch, m_disintegrate.m_pitchRandomizer);
    }

}
