using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    
    [System.Serializable] public class Sounds
    {
        [Header("References")]
        public AudioSource m_audioSource;

        [Header("Sounds")]
        public AudioClip[] m_sounds;

        [Header("Volume")]
        [Range(0, 1)] public float m_volume = 0.75f;
        [Range(0, 0.5f)] public float m_volumeRandomizer = 0.2f;

        [Header("Pitch")]
        [Range(-3, 3)] public float m_pitch = 1;
        [Range(0, 1.5f)] public float m_pitchRandomizer = 0.1f;
    }

    protected AudioClip GetAudioFromArray(AudioClip[] audios)
    {
        if (audios.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return null;
        }
        
        return audios[Random.Range(0, audios.Length)];
    }

    protected float GetRandomValue(float baseValue, float randomizerRange)
    {
		return baseValue - Random.Range(-randomizerRange, randomizerRange);
    }

    protected void StartSoundFromArray(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer)
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
    
}
