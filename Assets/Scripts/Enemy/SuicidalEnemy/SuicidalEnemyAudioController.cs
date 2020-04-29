using System;
using System.Collections;
using UnityEngine;

public class SuicidalEnemyAudioController : AudioController
{

    [SerializeField] MovementSoundLooper m_movements;
    [SerializeField] Sounds m_activateSelfDestruction;
    [SerializeField] Sounds m_selfDestructionBIP;
    [SerializeField] Sounds m_explode;
    [SerializeField] Sounds m_die;
    [SerializeField] Sounds m_disintegrate;

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
    public void On_EnemyWaitToExplode()
    {
        StartSoundFromArray(m_selfDestructionBIP.m_audioSource, m_selfDestructionBIP.m_sounds, m_selfDestructionBIP.m_volume, m_selfDestructionBIP.m_volumeRandomizer, m_selfDestructionBIP.m_pitch, m_selfDestructionBIP.m_pitchRandomizer);
    }
    public void On_EnemyExplode()
    {
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
