using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeShaderValue : ChangeValues
{
    
    [SerializeField] float m_timeToFadeIn = 0.25f, m_timeToFadeOff = 0.5f;
    [SerializeField, Range(0, 1)] float m_fromValue, m_toValue;
    [SerializeField] bool m_useCurve = false;
    [SerializeField] AnimationCurve m_curve;

    [Header("Blink")]
    [SerializeField] bool m_useBlink = true;
    [SerializeField, Range(0, 1)] float m_minValue = 0, m_maxValue = 1;
    [SerializeField] float m_timeToChangeValue = 1;
    [SerializeField] float m_waitTimeBetweenValue = 0;

    Image m_targetImage;
    float m_distanceFromTargetedValues;
    float m_speedToFadeIn, m_speedToFadeOff;
    bool m_blinkIsActivate = false;
    IEnumerator m_currentBlinkAnim;
    bool m_isBlinkOn = true;

    void Awake()
    {
        m_targetImage = GetComponent<Image>();

        m_distanceFromTargetedValues = GetDistanceFromValues(m_fromValue, m_toValue);
        m_speedToFadeIn = m_distanceFromTargetedValues / m_timeToFadeIn;
        m_speedToFadeOff = m_distanceFromTargetedValues / m_timeToFadeOff;
    }
    protected override void SetupChangeValue(bool startWithFromValue)
    {
        base.SetupChangeValue(startWithFromValue);

        if (startWithFromValue)
            SetShaderValue(m_fromValue);
        else
            SetShaderValue(m_toValue);
    }

    public override void SwitchValue()
    {
        base.SwitchValue();
        if (m_needToFadeIn)
        {
            if (m_useBlink)
                m_blinkIsActivate = true;
            CheckToStartChangeImageColorCoroutine(m_toValue, m_speedToFadeIn);
        }
        else
        {
            if (m_useBlink)
            {
                m_blinkIsActivate = false;
                StartToBlink(false);
            }
            CheckToStartChangeImageColorCoroutine(m_fromValue, m_speedToFadeOff);
        }
    }

    void CheckToStartChangeImageColorCoroutine(float toValue, float speed)
    {
        if (m_currentChangementValues != null)
            StopCoroutine(m_currentChangementValues);

        m_currentChangementValues = ChangeShaderValueCorout(toValue, speed);
        StartCoroutine(m_currentChangementValues);
    }

    IEnumerator ChangeShaderValueCorout(float toValue, float speed)
    {
        m_valueIsChanging = true;

        float fromValue = GetShaderValue();

        float actualValue = fromValue;
        float fracJourney = 0;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / m_distanceFromTargetedValues;
            if (m_useCurve)
                actualValue = Mathf.Lerp(fromValue, toValue, m_curve.Evaluate(fracJourney));
            else
                actualValue = Mathf.Lerp(fromValue, toValue, fracJourney);
            SetShaderValue(actualValue);
            yield return null;
        }
        if (m_useBlink && m_blinkIsActivate)
            StartToBlink(true);
        else
            m_valueIsChanging = false;
    }

    void StartToBlink(bool blink)
    {
        if (m_currentBlinkAnim != null)
            StopCoroutine(m_currentBlinkAnim);

        if (blink)
        {
            m_currentBlinkAnim = BlinkImageColor();
            StartCoroutine(m_currentBlinkAnim);
        }
    }
    IEnumerator BlinkImageColor()
    {
        m_isBlinkOn =! m_isBlinkOn;

        if (m_isBlinkOn)
            yield return new WaitForSeconds(m_waitTimeBetweenValue);

        float fromValue = GetShaderValue();
        float toValue = m_isBlinkOn ? m_maxValue : m_minValue;

        float distance = GetDistanceFromValues(fromValue, toValue);
        float speed = distance / m_timeToChangeValue;

        float actualValue = fromValue;
        float fracJourney = 0;

        while (actualValue != toValue && m_useBlink && m_blinkIsActivate)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, fracJourney);
            SetShaderValue(actualValue);
            yield return null;
        }
        if (m_useBlink && m_blinkIsActivate)
            StartCoroutine(BlinkImageColor());
        else
            m_valueIsChanging = false;
    }

    public override void StopChangingValues()
    {
        // m_blinkIsActivate = false;
        StartToBlink(false);
        base.StopChangingValues();
    }

    [Header("Use for Debug")]
    [SerializeField] Image image;
    float GetShaderValue()
    {
        return image.color.a;
    }
    void SetShaderValue(float newValue)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, newValue);
    }

}
