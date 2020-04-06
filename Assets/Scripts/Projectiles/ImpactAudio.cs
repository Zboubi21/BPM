using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ImpactAudio : MonoBehaviour
{

    [Header("SFX")]
    public AudioSource hitSource;

    public AllDifferentHitSound allDifferentHitSound;
    [Serializable]
    public class AllDifferentHitSound
    {
        public AudioClip[] allDifferentClip;
        public Pitch Pitch;
        public Volume Volume;
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

    private void Start()
    {
        StartSoundFromArray(hitSource, allDifferentHitSound.allDifferentClip, allDifferentHitSound.Volume.volume, allDifferentHitSound.Volume.volumeRandomizer, allDifferentHitSound.Pitch.pitch, allDifferentHitSound.Pitch.pitchRandomizer);
    }


    void StartSoundFromArray(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer)
    {
        if (audioClip.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return;
        }

        AudioClip sound = GetAudioFromArray(audioClip);
        float volumeValue = GetRandomValue(volume, volumeRandomizer);
        float pitchValue = GetRandomValue(pitch, pitchRandomizer);

        audioSource.volume = volumeValue;
        audioSource.pitch = pitchValue;

        audioSource.PlayOneShot(sound);
    }

    AudioClip GetAudioFromArray(AudioClip[] audios)
    {
        if (audios.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return null;
        }

        return audios[UnityEngine.Random.Range(0, audios.Length)];
    }

    float GetRandomValue(float baseValue, float randomizerRange)
    {
        return baseValue - UnityEngine.Random.Range(-randomizerRange, randomizerRange);
    }
}
