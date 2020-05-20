﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeShaderValue : ChangeValues
{
    
    [Header("Shader")]
    [SerializeField] bool m_isImageShader = true;
    [SerializeField] protected string m_shaderParameter;
    [SerializeField] protected int m_materialNbr = 0;

    [Header("Parameters")]
    [SerializeField] protected float m_timeToFadeIn = 0.25f;
    [SerializeField] protected float m_timeToFadeOff = 0.5f;
    [SerializeField] float m_fromValue, m_toValue;
    protected float m_currentFromValue, m_currentToValue;
    [SerializeField] bool m_useCurve = false;
    [SerializeField] protected AnimationCurve m_curve;

    [Header("Blink")]
    [SerializeField] bool m_useBlink = true;
    [SerializeField, Range(0, 1)] float m_minValue = 0, m_maxValue = 1;
    [SerializeField] float m_timeToChangeValue = 1;
    [SerializeField] float m_waitTimeBetweenValue = 0;

    Material m_shader;
    protected float m_distanceFromTargetedValues;
    protected float m_speedToFadeIn, m_speedToFadeOff;
    bool m_blinkIsActivate = false;
    IEnumerator m_currentBlinkAnim;
    bool m_isBlinkOn = true;

    protected virtual void Awake()
    {
        if (m_isImageShader)
        {
            Image image = GetComponent<Image>();
            if (image != null)
                m_shader = image.material;

            RawImage rawImage = GetComponent<RawImage>();
            if (rawImage != null)
                m_shader = rawImage.material;
        }
        else
        {
            MeshRenderer mesh = GetComponent<MeshRenderer>();
            if (mesh != null)
                m_shader = mesh.materials[m_materialNbr];
        }
        SetupVariables();
    }
    
    protected virtual void SetupVariables()
    {
        m_currentFromValue = m_fromValue;
        m_currentToValue = m_toValue;

        m_distanceFromTargetedValues = GetDistanceFromValues(m_currentFromValue, m_currentToValue);
        m_speedToFadeIn = m_distanceFromTargetedValues / m_timeToFadeIn;
        m_speedToFadeOff = m_distanceFromTargetedValues / m_timeToFadeOff;
    }
    protected override void SetupChangeValue(bool startWithFromValue)
    {
        base.SetupChangeValue(startWithFromValue);

        if (startWithFromValue)
            SetShaderValue(m_currentFromValue);
        else
            SetShaderValue(m_currentToValue);
    }

    public override void SwitchValue()
    {
        base.SwitchValue();
        if (m_needToFadeIn)
        {
            if (m_useBlink)
                m_blinkIsActivate = true;
            CheckToStartChangeImageColorCoroutine(m_currentToValue, m_speedToFadeIn);
        }
        else
        {
            if (m_useBlink)
            {
                m_blinkIsActivate = false;
                StartToBlink(false);
            }
            CheckToStartChangeImageColorCoroutine(m_currentFromValue, m_speedToFadeOff);
        }
    }
    public override void SwitchValue(bool newValue)
    {
        base.SwitchValue(newValue);
        if (m_needToFadeIn)
        {
            if (m_useBlink)
                m_blinkIsActivate = true;
            CheckToStartChangeImageColorCoroutine(m_currentToValue, m_speedToFadeIn);
        }
        else
        {
            if (m_useBlink)
            {
                m_blinkIsActivate = false;
                StartToBlink(false);
            }
            CheckToStartChangeImageColorCoroutine(m_currentFromValue, m_speedToFadeOff);
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
        On_ChangeShaderValueCoroutIsDone();
    }
    protected virtual void On_ChangeShaderValueCoroutIsDone()
    {
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

    // [Header("Use for Debug")]
    // [SerializeField] Image image;
    protected virtual float GetShaderValue()
    {
        return m_shader.GetFloat(m_shaderParameter);
    }
    protected virtual float GetShaderValue(string shaderParameter)
    {
        return m_shader.GetFloat(shaderParameter);
    }
    protected virtual void SetShaderValue(float newValue)
    {
        m_shader?.SetFloat(m_shaderParameter, newValue);
    }
    protected virtual void SetShaderValue(string shaderParameter, float newValue)
    {
        m_shader?.SetFloat(shaderParameter, newValue);
    }

    protected void SetShaderBool(Material shader, string parameters, bool b)
    {
        int boolValue = b ? 1 : 0;
        shader.SetInt(parameters, boolValue);
    }
    protected void SetShaderBool(Material[] shader, string parameters, bool b)
    {
        for (int i = 0, l = shader.Length; i < l; ++i)
        {
            int boolValue = b ? 1 : 0;
            shader[i].SetInt(parameters, boolValue);
        }
    }

}
