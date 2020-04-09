using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLooper : AudioController
{
    
    [SerializeField] AudioSource m_loopSource;

    [Header("Before Loop")]
    [SerializeField] LoopSound m_beforeLoop;

    [Header("Loop")]
    [SerializeField] float m_delayToStartLoop = 1;
    [SerializeField] FadeSound m_fadeIn;
    [SerializeField] FadeSound m_fadeOut;
    [SerializeField] float m_delayToStartNextSound = 1;

    [Header("After Loop")]
    [SerializeField] LoopSound m_afterLoop;

    [System.Serializable] class LoopSound
    {
        public bool m_useSound = false;
        public AudioSource m_audioSource;
        public Sound m_sound;
    }

    [System.Serializable] class FadeSound
    {
        public bool m_useFade = false;
        public float m_timeToFade = 0.125f; // From 0 -> 1 OR 0 <- 1
        [Range(0, 1)] public float m_volume = 1;
        public AnimationCurve m_fadeCurve;
        [HideInInspector] public float m_fadeSpeed = 0;
    }

    void Start()
    {
        m_fadeIn.m_fadeSpeed = 1 / m_fadeIn.m_timeToFade;
        m_fadeOut.m_fadeSpeed = 1 / m_fadeOut.m_timeToFade;
    }

    public void On_StartLoop(bool start)
    {
        if (start)
        {
            float delay = m_beforeLoop.m_useSound ? m_delayToStartLoop : 0;
            StartCoroutine(StartLoop(start, delay));

            if (m_beforeLoop.m_useSound)
                StartSound(m_beforeLoop.m_audioSource, m_beforeLoop.m_sound.m_sound, m_beforeLoop.m_sound.m_volume, m_beforeLoop.m_sound.m_pitch);
        }
        else
        {
            StartCoroutine(StartLoop(start, 0));

            if (m_afterLoop.m_useSound)
                StartSound(m_afterLoop.m_audioSource, m_afterLoop.m_sound.m_sound, m_afterLoop.m_sound.m_volume, m_afterLoop.m_sound.m_pitch, m_delayToStartNextSound);
        }
    }
    IEnumerator StartLoop(bool start, float delayToStart)
    {
        yield return new WaitForSeconds(delayToStart);
        if (start)
        {
            m_loopSource.Play();
            if (m_fadeIn.m_useFade)
                StartCoroutine(FadeVolume(m_loopSource, m_fadeIn.m_volume, m_fadeIn.m_fadeSpeed, m_fadeIn.m_fadeCurve));
            else
                SetAudioSource(m_loopSource, m_fadeIn.m_volume);
        }
        else
        {
            if (m_fadeOut.m_useFade)
                StartCoroutine(FadeVolume(m_loopSource, 0, m_fadeOut.m_fadeSpeed, m_fadeOut.m_fadeCurve, true));
            else
                SetAudioSource(m_loopSource, 0, true);
        }
    }

    IEnumerator FadeVolume(AudioSource source, float toValue, float speed, AnimationCurve curve, bool stopSoundAfterFade = false)
    {
        float fromValue = source.volume;
        float actualValue = fromValue;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue - toValue);

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, curve.Evaluate(fracJourney));
            SetAudioSource(source, actualValue);
            yield return null;
        }
        if (stopSoundAfterFade)
            source.Stop();
    }

    void SetAudioSource(AudioSource source, float volume, bool stopSound = false)
    {
        source.volume = volume;
        if (stopSound)
            source.Stop();
    }
    
}
