using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GroundType;
using System;

public class EnemyAudioController : AudioController
{
    #region SerializeField Functions
    [Header("Spawn")]
    [SerializeField] Sounds m_spawn;

    [Header("Steps")]
    [SerializeField] Steps m_steps;
    [System.Serializable]
    class Steps
    {
        public float m_footStepDistance = 0.25f;
        public float m_footStepDistanceRandomizer = 0.025f;
        public Sounds m_stoneStep;
        public Sounds m_woodStep;
        public Sounds m_glassStep;
    }
    [Header("WeaponSound")]
    public WeaponSound weaponSound;
    [Serializable]
    public class WeaponSound
    {
        public AudioSource source;
        [Space]
        public AllDifferentFireSound FireSounds;
        [Serializable]
        public class AllDifferentFireSound
        {
            public AudioClip[] allDifferentClip;
            public Pitch Pitch;
            public Volume Volume;
        }
        [Space]
        public AllDifferentLastFireSound LastFireSounds;
        [Serializable]
        public class AllDifferentLastFireSound
        {
            public AudioClip[] allDifferentClip;
            public Pitch Pitch;
            public Volume Volume;
        }
    }

    #region Serializable class
    [Serializable]
    public class Pitch
    {
        [Range(0.1f, 1f)]
        public float pitch;
        float _currentPitch;
        [Space]
        [Range(0, 1)]
        public float pitchRandomizer;
        public float CurrentPitch { get => _currentPitch; set => _currentPitch = value; }
    }
    [Serializable]
    public class Volume
    {
        [Range(0, 1)]
        public float volume;
        float _currentVolume;
        [Space]
        [Range(0, 1)]
        public float volumeRandomizer;
        public float CurrentVolume { get => _currentVolume; set => _currentVolume = value; }
    }
    #endregion

    [Header("Steps & Land Raycast")]
    [SerializeField] Transform m_raycastTrans;
    [SerializeField] float m_raycastMaxDistance = 0.25f;
    [SerializeField] LayerMask m_groundMask;

    [Header("Die")]
    [SerializeField] SoundsWithDelay m_die;
    [SerializeField] SoundsWithDelay m_disintegrate;
    
    [Header("Gizmos")]
    [SerializeField] bool m_showGizmos = false;
    [SerializeField] Color m_gizmosColor = Color.magenta;
    #endregion

    #region Private Variables
    bool m_isRunning = false;
    float m_targetedFootStepDistance;
    float m_currentFootStepDistance = 0f;
    #endregion

    #region Event Functions
    void Start()
    {
        SetTargetedFootStepDistance();
    }
    void Update()
    {
        if (m_isRunning)
            CheckFootSteps();
    }
    void OnDrawGizmos()
    {
        if (!m_showGizmos)
            return;

        Gizmos.color = m_gizmosColor;
        Gizmos.DrawLine(m_raycastTrans.position, m_raycastTrans.position + m_raycastTrans.forward * m_raycastMaxDistance);
    }
    #endregion

    #region Private Functions
    void CheckFootSteps()
    {
        m_currentFootStepDistance += Time.deltaTime;
        if (m_currentFootStepDistance > m_targetedFootStepDistance)
        {
            PlayFootStepSound();
            m_currentFootStepDistance = 0;
            SetTargetedFootStepDistance();
        }
    }
    void PlayFootStepSound()
    {
        switch (CheckEnemyGround())
        {
            case GroundTypeEnum.Stone:
                StartSoundFromArray(m_steps.m_stoneStep.m_audioSource, m_steps.m_stoneStep.m_sounds, m_steps.m_stoneStep.m_volume, m_steps.m_stoneStep.m_volumeRandomizer, m_steps.m_stoneStep.m_pitch, m_steps.m_stoneStep.m_pitchRandomizer);
                break;
            case GroundTypeEnum.Wood:
                StartSoundFromArray(m_steps.m_woodStep.m_audioSource, m_steps.m_woodStep.m_sounds, m_steps.m_woodStep.m_volume, m_steps.m_woodStep.m_volumeRandomizer, m_steps.m_woodStep.m_pitch, m_steps.m_woodStep.m_pitchRandomizer);
                break;
            case GroundTypeEnum.Glass:
                StartSoundFromArray(m_steps.m_glassStep.m_audioSource, m_steps.m_glassStep.m_sounds, m_steps.m_glassStep.m_volume, m_steps.m_glassStep.m_volumeRandomizer, m_steps.m_glassStep.m_pitch, m_steps.m_glassStep.m_pitchRandomizer);
                break;
        }
    }
    GroundTypeEnum CheckEnemyGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_raycastTrans.position, m_raycastTrans.forward, out hit, m_raycastMaxDistance, m_groundMask))
        {
            Ground hitGround = hit.collider.GetComponent<Ground>();
            if (hitGround != null)
                return hitGround.GroundType;
        }
        return GroundTypeEnum.Stone;
    }
    void SetTargetedFootStepDistance()
    {
        m_targetedFootStepDistance = GetRandomValue(m_steps.m_footStepDistance, m_steps.m_footStepDistanceRandomizer);
    }
    #endregion

    #region Public Functions
    public void On_Spawn()
    {
        StartSoundFromArray(m_spawn.m_audioSource, m_spawn.m_sounds, m_spawn.m_volume, m_spawn.m_volumeRandomizer, m_spawn.m_pitch, m_spawn.m_pitchRandomizer);
    }
    public void On_Run(bool isRunning)
    {
        m_isRunning = isRunning;
        if (m_isRunning)
        {
            m_currentFootStepDistance = 0;
            SetTargetedFootStepDistance();
        }
    }
    public void PlayAppropriateLastFireSound()
    {
        if (weaponSound.source != null && weaponSound.LastFireSounds.allDifferentClip.Length > 0)
        {
            StartSoundFromArray(weaponSound.source, weaponSound.LastFireSounds.allDifferentClip, weaponSound.LastFireSounds.Volume.volume, weaponSound.LastFireSounds.Volume.volumeRandomizer, weaponSound.LastFireSounds.Pitch.pitch, weaponSound.LastFireSounds.Pitch.pitchRandomizer);
        }
    }

    public void PlayAppropriateFireSound()
    {
        if (weaponSound.source != null && weaponSound.FireSounds.allDifferentClip.Length > 0)
        {
            StartSoundFromArray(weaponSound.source, weaponSound.FireSounds.allDifferentClip, weaponSound.FireSounds.Volume.volume, weaponSound.FireSounds.Volume.volumeRandomizer, weaponSound.FireSounds.Pitch.pitch, weaponSound.FireSounds.Pitch.pitchRandomizer);
        }
    }

    public void On_EnemyDie()
    {
        StartSoundFromArrayWithDelay(m_die.m_sounds.m_audioSource, m_die.m_sounds.m_sounds, m_die.m_sounds.m_volume, m_die.m_sounds.m_volumeRandomizer, m_die.m_sounds.m_pitch, m_die.m_sounds.m_pitchRandomizer, m_die.m_delayToStartSound);
    }
    public void On_EnemyIsDisintegrate()
    {
        StartSoundFromArrayWithDelay(m_disintegrate.m_sounds.m_audioSource, m_disintegrate.m_sounds.m_sounds, m_disintegrate.m_sounds.m_volume, m_disintegrate.m_sounds.m_volumeRandomizer, m_disintegrate.m_sounds.m_pitch, m_disintegrate.m_sounds.m_pitchRandomizer, m_disintegrate.m_delayToStartSound);
    }
    #endregion
}
