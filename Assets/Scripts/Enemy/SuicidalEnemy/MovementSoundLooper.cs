using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSoundLooper : SoundLooper
{
    
    [Header("Pitch")]
    [SerializeField] Pitchs m_beforeLoopPitch;
    [SerializeField] Pitchs m_loopPitch;
    [SerializeField] Pitchs m_afterLoopPitch;
    [Serializable] class Pitchs
    {
        [Range(-3, 3)] public float m_pitchValue = 1.25f;
        public float m_timeToChange = 0.5f;
        public AnimationCurve m_changeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    }

    float m_startBeforeLoopFromPitch;
    float m_startLoopFromPitch;
    float m_startAfterLoopFromPitch;

    void Awake()
    {
        m_startBeforeLoopFromPitch = m_beforeLoop.m_sound.m_pitch;
        m_startLoopFromPitch = m_loopSource.pitch;
        m_startAfterLoopFromPitch = m_afterLoop.m_sound.m_pitch;
    }

    public void SwitchPitch()
    {
        StartCoroutine(SwitchBeforeLoopPitch(m_startBeforeLoopFromPitch, m_beforeLoopPitch.m_pitchValue, m_beforeLoopPitch.m_timeToChange, m_beforeLoopPitch.m_changeCurve));
        StartCoroutine(SwitchPitch(m_loopSource, m_loopPitch.m_pitchValue, m_loopPitch.m_timeToChange, m_loopPitch.m_changeCurve));
        StartCoroutine(SwitchAfterLoopPitch(m_startAfterLoopFromPitch, m_afterLoopPitch.m_pitchValue, m_afterLoopPitch.m_timeToChange, m_afterLoopPitch.m_changeCurve));
    }
    IEnumerator SwitchBeforeLoopPitch(float fromValue, float toValue, float timeToChange, AnimationCurve curve)
    {
        float actualValue = fromValue;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue - toValue);
        float speed = distance / timeToChange;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, curve.Evaluate(fracJourney));
            m_beforeLoop.m_sound.m_pitch = actualValue;
            yield return null;
        }
    }
    IEnumerator SwitchPitch(AudioSource source, float toValue, float timeToChange, AnimationCurve curve)
    {
        float fromValue = source.pitch;
        float actualValue = fromValue;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue - toValue);
        float speed = distance / timeToChange;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, curve.Evaluate(fracJourney));
            source.pitch = actualValue;
            yield return null;
        }
    }
    IEnumerator SwitchAfterLoopPitch(float fromValue, float toValue, float timeToChange, AnimationCurve curve)
    {
        float actualValue = fromValue;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue - toValue);
        float speed = distance / timeToChange;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, curve.Evaluate(fracJourney));
            m_afterLoop.m_sound.m_pitch = actualValue;
            yield return null;
        }
    }

    public void SetStartPitch()
    {
        m_beforeLoop.m_sound.m_pitch = m_startBeforeLoopFromPitch;
        m_loopSource.pitch = m_startLoopFromPitch;
        m_afterLoop.m_sound.m_pitch = m_startAfterLoopFromPitch;
    }

}
