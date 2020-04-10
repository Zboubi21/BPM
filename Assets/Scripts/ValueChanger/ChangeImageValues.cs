using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageValues : ChangeValues
{
    
    [SerializeField] float m_timeToFadeIn = 0.25f, m_timeToFadeOff = 0.5f;
    [SerializeField] Color m_fromColor, m_toColor;
    [SerializeField] bool m_useCurve = false;
    [SerializeField] AnimationCurve m_curve;

    [Header("Blink")]
    [SerializeField] bool m_useBlink = true;
    [SerializeField, Range(0, 255)] float m_minAlpha = 0, m_maxAlpha = 255;
    [SerializeField] float m_timeToChangeAlpha = 1;

    Image m_targetImage;
    float m_distanceFromTargetedColors;
    float m_speedToFadeIn, m_speedToFadeOff;
    bool m_blinkIsActivate = false;
    IEnumerator m_currentBlinkAnim;
    bool m_isBlinkOn = true;

    void Awake()
    {
        m_targetImage = GetComponent<Image>();

        m_distanceFromTargetedColors = GetDistanceFromColors(m_fromColor, m_toColor);
        m_speedToFadeIn = m_distanceFromTargetedColors / m_timeToFadeIn;
        m_speedToFadeOff = m_distanceFromTargetedColors / m_timeToFadeOff;
    }
    protected override void SetupChangeValue(bool startWithFromValue)
    {
        base.SetupChangeValue(startWithFromValue);

        if (startWithFromValue)
            SetImageColor(m_targetImage, m_fromColor);
        else
            SetImageColor(m_targetImage, m_toColor);
    }

    public override void SwitchValue()
    {
        base.SwitchValue();
        if (m_needToFadeIn)
        {
            if (m_useBlink)
                m_blinkIsActivate = true;
            CheckToStartChangeImageColorCoroutine(m_toColor, m_speedToFadeIn);
        }
        else
        {
            if (m_useBlink)
            {
                m_blinkIsActivate = false;
                StartToBlink(false);
            }
            CheckToStartChangeImageColorCoroutine(m_fromColor, m_speedToFadeOff);
        }
    }

    void CheckToStartChangeImageColorCoroutine(Color toColor, float speed)
    {
        if (m_currentChangementValues != null)
            StopCoroutine(m_currentChangementValues);

        m_currentChangementValues = ChangeImageColor(toColor, speed);
        StartCoroutine(m_currentChangementValues);
    }

    IEnumerator ChangeImageColor(Color toColor, float speed)
    {
        m_valueIsChanging = true;

        Color fromColor = m_targetImage.color;

        Color actualColor = fromColor;
        float fracJourney = 0;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / m_distanceFromTargetedColors;
            if (m_useCurve)
                actualColor = Color.Lerp(fromColor, toColor, m_curve.Evaluate(fracJourney));
            else
                actualColor = Color.Lerp(fromColor, toColor, fracJourney);
            SetImageColor(m_targetImage, actualColor);
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

        Color fromColor = m_targetImage.color;
        Color toColor = m_isBlinkOn ? new Color(fromColor.r, fromColor.g, fromColor.b, m_maxAlpha / 255) : new Color(fromColor.r, fromColor.g, fromColor.b, m_minAlpha / 255);

        float distance = GetDistanceFromColors(fromColor, toColor);
        float speed = distance / m_timeToChangeAlpha;

        Color actualColor = fromColor;
        float fracJourney = 0;

        while (actualColor != toColor && m_useBlink && m_blinkIsActivate)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualColor = Color.Lerp(fromColor, toColor, fracJourney);
            SetImageColor(m_targetImage, actualColor);
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

}
