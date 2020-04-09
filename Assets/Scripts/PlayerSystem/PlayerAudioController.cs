
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GroundType;
using System;

public class PlayerAudioController : AudioController
{

#region SerializeField Functions
    [Header("Steps")]
    [SerializeField] Steps m_steps;
    [System.Serializable] class Steps
    {
        public float m_footStepDistance = 0.25f;
        public float m_footStepDistanceRandomizer = 0.025f;
        public Sounds m_stoneStep;
        public Sounds m_woodStep;
        public Sounds m_glassStep;
    }

    [Header("Jump")]
    [SerializeField] Sounds m_jump;

    [Header("Double Jump")]
    [SerializeField] Sounds m_doubleJump;

    [Header("Land")]
    [SerializeField] Land m_land;
    [System.Serializable] class Land
    {
        public Sounds m_stone;
        public Sounds m_wood;
        public Sounds m_glass;
    }

    [Header("Dash")]
    [SerializeField] Sounds m_dash;

    [Header("BPM")]
    [SerializeField] BPM m_bpm;
    [Serializable] class BPM
    {
        public Sounds m_sounds;

        [Header("Timers beetween sounds")]
        public float m_waitTimeBetweenHeart = 0.4f;
        public float m_waitTimeAfterNextHeart = 0.8f;
    }

    [Header("Overadrenaline")]
    [SerializeField] SoundLooper m_overadrenaline;

    [Header("WeaponSound")]
    public WeaponSound weaponSound;
    [Serializable]
    public class WeaponSound
    {
        public AudioSource fireSource;
        [Space]
        public AllDifferentFireSound[] allDifferentFireSound;
        [Serializable]
        public class AllDifferentFireSound
        {
            public AudioClip[] allDifferentClip;
            public Pitch Pitch;
            public Volume Volume;
        }
        [Space]
        public AllDifferentLastFireSound[] allDifferentLastFireSound;
        [Serializable]
        public class AllDifferentLastFireSound
        {
            public AudioClip[] allDifferentClip;
            public Pitch Pitch;
            public Volume Volume;
        }
        [Space]
        public AudioSource morphSource;
        [Space]
        public AllDifferentDegradeSound[] allDifferentDegradeSound;
        [Serializable]
        public class AllDifferentDegradeSound
        {
            public AudioClip[] allDifferentClip;
            public Pitch Pitch;
            public Volume Volume;
        }
        public AllDifferentUpgradeSound[] allDifferentUpgradeSound;
        [Serializable]
        public class AllDifferentUpgradeSound
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

    [Header("Gizmos")]
    [SerializeField] bool m_showGizmos = false;
    [SerializeField] Color m_gizmosColor = Color.magenta;
#endregion

#region Private Variables
    bool m_isRunning = false;
    float m_targetedFootStepDistance;
	float m_currentFootStepDistance = 0f;
    bool m_isOnCriticalBpm = false;
    bool m_isOnOveradrenaline = false;
#endregion

#region Event Functions
    void Start()
    {
        SetTargetedFootStepDistance();
    }
	void FixedUpdate () {
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
        if (!m_isRunning)
            return;
        
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
        switch (CheckPlayerGround())
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
    GroundTypeEnum CheckPlayerGround()
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
    public void On_Run(bool isRunning)
    {
        m_isRunning = isRunning;
        if (m_isRunning)
        {
            m_currentFootStepDistance = 0;
            SetTargetedFootStepDistance();
        }
    }
    public void On_Jump()
    {
        StartSoundFromArray(m_jump.m_audioSource, m_jump.m_sounds, m_jump.m_volume, m_jump.m_volumeRandomizer, m_jump.m_pitch, m_jump.m_pitchRandomizer);
    }
    public void On_DoubleJump()
    {
        StartSoundFromArray(m_doubleJump.m_audioSource, m_doubleJump.m_sounds, m_doubleJump.m_volume, m_doubleJump.m_volumeRandomizer, m_doubleJump.m_pitch, m_doubleJump.m_pitchRandomizer);
    }
    public void On_Land()
    {
        switch (CheckPlayerGround())
        {
            case GroundTypeEnum.Stone:
                StartSoundFromArray(m_land.m_stone.m_audioSource, m_land.m_stone.m_sounds, m_land.m_stone.m_volume, m_land.m_stone.m_volumeRandomizer, m_land.m_stone.m_pitch, m_land.m_stone.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Wood:
                StartSoundFromArray(m_land.m_wood.m_audioSource, m_land.m_wood.m_sounds, m_land.m_wood.m_volume, m_land.m_wood.m_volumeRandomizer, m_land.m_wood.m_pitch, m_land.m_wood.m_pitchRandomizer);
            break;
            case GroundTypeEnum.Glass:
                StartSoundFromArray(m_land.m_glass.m_audioSource, m_land.m_glass.m_sounds, m_land.m_glass.m_volume, m_land.m_glass.m_volumeRandomizer, m_land.m_glass.m_pitch, m_land.m_glass.m_pitchRandomizer);
            break;
        }
    }
    public void On_Dash()
    {
        StartSoundFromArray(m_dash.m_audioSource, m_dash.m_sounds, m_dash.m_volume, m_dash.m_volumeRandomizer, m_dash.m_pitch, m_dash.m_pitchRandomizer);
    }
    public void PlayAppropriateLastFireSound(int currentIndex)
    {
        if (weaponSound.allDifferentLastFireSound.Length == (int)BPMSystem.WeaponState.Fury + 1 && weaponSound.fireSource != null)
        {
            StartSoundFromArray(weaponSound.fireSource, weaponSound.allDifferentLastFireSound[currentIndex].allDifferentClip, weaponSound.allDifferentLastFireSound[currentIndex].Volume.volume, weaponSound.allDifferentLastFireSound[currentIndex].Volume.volumeRandomizer, weaponSound.allDifferentLastFireSound[currentIndex].Pitch.pitch, weaponSound.allDifferentLastFireSound[currentIndex].Pitch.pitchRandomizer);
        }
    }

    public void On_CriticalBpm(bool criticalBpm)
    {
        m_isOnCriticalBpm = criticalBpm;
        if (m_isOnCriticalBpm)
            StartCoroutine(StartFirstHeartSound());
    }
    IEnumerator StartFirstHeartSound()
    {
        StartSound(m_bpm.m_sounds.m_audioSource, m_bpm.m_sounds.m_sounds[0], m_bpm.m_sounds.m_volume, m_bpm.m_sounds.m_volumeRandomizer, m_bpm.m_sounds.m_pitch, m_bpm.m_sounds.m_pitchRandomizer);
        yield return new WaitForSeconds(m_bpm.m_waitTimeBetweenHeart);
        if (m_isOnCriticalBpm)
            StartCoroutine(StartSecondHeartSound());
    }
    IEnumerator StartSecondHeartSound()
    {
        StartSound(m_bpm.m_sounds.m_audioSource, m_bpm.m_sounds.m_sounds[1], m_bpm.m_sounds.m_volume, m_bpm.m_sounds.m_volumeRandomizer, m_bpm.m_sounds.m_pitch, m_bpm.m_sounds.m_pitchRandomizer);
        yield return new WaitForSeconds(m_bpm.m_waitTimeAfterNextHeart);
        if (m_isOnCriticalBpm)
            StartCoroutine(StartFirstHeartSound());
    }

    public void On_Overadrenaline(bool isActivate)
    {
        m_overadrenaline.On_StartLoop(isActivate);
    }

    public void PlayAppropriateFireSound(int currentIndex)
    {
        if (weaponSound.allDifferentFireSound.Length == (int)BPMSystem.WeaponState.Fury + 1 && weaponSound.fireSource != null)
        {
            StartSoundFromArray(weaponSound.fireSource, weaponSound.allDifferentFireSound[currentIndex].allDifferentClip, weaponSound.allDifferentFireSound[currentIndex].Volume.volume, weaponSound.allDifferentFireSound[currentIndex].Volume.volumeRandomizer, weaponSound.allDifferentFireSound[currentIndex].Pitch.pitch, weaponSound.allDifferentFireSound[currentIndex].Pitch.pitchRandomizer);
        }
    }

    public void PlayWeaponUpgradeSound(int levelToUpgrade)
    {
        if (weaponSound.allDifferentUpgradeSound.Length == (int)BPMSystem.WeaponState.Fury && weaponSound.morphSource != null)
        {
            StartSoundFromArray(weaponSound.morphSource, weaponSound.allDifferentUpgradeSound[levelToUpgrade].allDifferentClip, weaponSound.allDifferentUpgradeSound[levelToUpgrade].Volume.volume, weaponSound.allDifferentUpgradeSound[levelToUpgrade].Volume.volumeRandomizer, weaponSound.allDifferentUpgradeSound[levelToUpgrade].Pitch.pitch, weaponSound.allDifferentUpgradeSound[levelToUpgrade].Pitch.pitchRandomizer);
            //Debug.Log(weaponSound.allDifferentUpgradeSound[levelToUpgrade].allDifferentClip[0]);
        }
    }

    public void PlayWeaponDegradeSound(int degradeFrom)
    {
        if (weaponSound.allDifferentDegradeSound.Length == (int)BPMSystem.WeaponState.Fury && weaponSound.morphSource != null)
        {
            StartSoundFromArray(weaponSound.morphSource, weaponSound.allDifferentDegradeSound[degradeFrom].allDifferentClip, weaponSound.allDifferentDegradeSound[degradeFrom].Volume.volume, weaponSound.allDifferentDegradeSound[degradeFrom].Volume.volumeRandomizer, weaponSound.allDifferentDegradeSound[degradeFrom].Pitch.pitch, weaponSound.allDifferentDegradeSound[degradeFrom].Pitch.pitchRandomizer);
            //Debug.Log(weaponSound.allDifferentDegradeSound[degradeFrom].allDifferentClip[0]);
        }
    }
    #endregion


}