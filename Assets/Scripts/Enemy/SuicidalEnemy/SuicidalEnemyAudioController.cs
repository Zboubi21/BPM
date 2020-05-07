using System;
using System.Collections;
using UnityEngine;

public class SuicidalEnemyAudioController : AudioController
{

    [SerializeField] Sounds m_spawn;
    [SerializeField] MovementSoundLooper m_movements;
    [SerializeField] Sounds m_activateSelfDestruction;
    [SerializeField] Sounds m_selfDestructionBIP;
    [SerializeField] Sounds m_explode;
    [SerializeField] SoundsWithDelay m_die;
    [SerializeField] SoundsWithDelay m_disintegrate;

    public void On_Spawn()
    {
        StartSoundFromArray(m_spawn.m_audioSource, m_spawn.m_sounds, m_spawn.m_volume, m_spawn.m_volumeRandomizer, m_spawn.m_pitch, m_spawn.m_pitchRandomizer);
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
    public void On_EnemyWaitToExplode()
    {
        StartSoundFromArray(m_selfDestructionBIP.m_audioSource, m_selfDestructionBIP.m_sounds, m_selfDestructionBIP.m_volume, m_selfDestructionBIP.m_volumeRandomizer, m_selfDestructionBIP.m_pitch, m_selfDestructionBIP.m_pitchRandomizer);
    }
    public void On_EnemyExplode()
    {
        m_movements.ResetAudioSource();
        StartSoundFromArray(m_explode.m_audioSource, m_explode.m_sounds, m_explode.m_volume, m_explode.m_volumeRandomizer, m_explode.m_pitch, m_explode.m_pitchRandomizer);
    }
    public void On_EnemyDie()
    {
        StartSoundFromArrayWithDelay(m_die.m_sounds.m_audioSource, m_die.m_sounds.m_sounds, m_die.m_sounds.m_volume, m_die.m_sounds.m_volumeRandomizer, m_die.m_sounds.m_pitch, m_die.m_sounds.m_pitchRandomizer, m_die.m_delayToStartSound);
    }
    public void On_EnemyIsDisintegrate()
    {
        StartSoundFromArrayWithDelay(m_disintegrate.m_sounds.m_audioSource, m_disintegrate.m_sounds.m_sounds, m_disintegrate.m_sounds.m_volume, m_disintegrate.m_sounds.m_volumeRandomizer, m_disintegrate.m_sounds.m_pitch, m_disintegrate.m_sounds.m_pitchRandomizer, m_disintegrate.m_delayToStartSound);
    }

}
