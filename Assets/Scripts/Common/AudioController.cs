using System.Collections;
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

        public int nbrOfPlayingSoundMax;
        public float timeOffset;
        int _currentNbrOfPlayedSound;

        public int CurrentNbrOfPlayedSound { get => _currentNbrOfPlayedSound; set => _currentNbrOfPlayedSound = value; }
    }

    [System.Serializable] public class SoundsWithDelay
    {
        public float m_delayToStartSound = 0.25f;
        public Sounds m_sounds;
    }

    [System.Serializable] public class SoundsWithPitchModifier
    {
        [Header("Pitch Modifier")]
        public float m_additionalPitchPerModifier = 0.1f;
        public int m_maxPitchModifier = 10;
        public float m_timeToResetpitch = 0.1f;

        [Header("Sounds")]
        public Sounds m_sounds;
    }

    [System.Serializable] public class Sound
    {
        public AudioClip m_sound;
        [Range(0, 1)] public float m_volume = 0.75f;
        [Range(-3, 3)] public float m_pitch = 1;
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

    protected void StartSoundFromArrayWithDelay(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer, float delay)
    {
        StartCoroutine(StartSoundFromArrayWithDelayCorout(audioSource, audioClip, volume, volumeRandomizer, pitch, pitchRandomizer, delay));
    }
    IEnumerator StartSoundFromArrayWithDelayCorout(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartSoundFromArray(audioSource, audioClip, volume, volumeRandomizer, pitch, pitchRandomizer);
    }

    protected void StartSound(AudioSource audioSource, AudioClip audioClip, float volume, float pitch, float delay = 0)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        if (delay == 0)
            audioSource.PlayOneShot(audioClip);
        else
            StartCoroutine(StartSoundWithDelay(audioSource, audioClip, delay));
    }
    IEnumerator StartSoundWithDelay(AudioSource audioSource, AudioClip audioClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(audioClip);
    }
    protected void StartSound(AudioSource audioSource, AudioClip audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer)
    {
        float volumeValue = GetRandomValue(volume, volumeRandomizer);
        float pitchValue = GetRandomValue(pitch, pitchRandomizer);

        audioSource.volume = volumeValue;
        audioSource.pitch = pitchValue;

        audioSource.PlayOneShot(audioClip);
    }

    protected void StopSound(AudioSource source)
    {
        source.Stop();
    }

}
