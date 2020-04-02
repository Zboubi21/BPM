using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ChangeScaleValues : ChangeValues
{
    
    [SerializeField] Vector3 m_minSizeAnim = new Vector3(0.75f, 0.75f, 1), m_maxSizeAnim = new Vector3(1.25f, 1.25f, 1);
    [SerializeField] float m_timeToDoMinSizeAnim = 1, m_timeToDoMaxSizeAnim = 1;
    [SerializeField] AnimationCurve m_curveAnim;

    RectTransform m_rectTrans;
    float m_speedToFadeIn, m_speedToFadeOff;
    float m_distanceToMinSize, m_distanceToMaxSize;
    Vector3 m_startScale;
    int m_passNbr = 0;

    protected override void Start()
    {
        m_rectTrans = GetComponent<RectTransform>();

        base.Start();

        m_startScale = m_rectTrans.localScale;
        m_distanceToMaxSize = GetDistanceFromVectors(m_startScale, m_maxSizeAnim);
        m_distanceToMinSize = GetDistanceFromVectors(m_startScale, m_minSizeAnim);
        m_speedToFadeIn = m_distanceToMaxSize / m_timeToDoMaxSizeAnim;
        m_speedToFadeOff = m_distanceToMinSize / m_timeToDoMinSizeAnim;
    }
    protected override void SetupChangeValue(bool startWithFromValue)
    {
        base.SetupChangeValue(startWithFromValue);
    }

    public override void SwitchValue(bool newValue)
    {
        base.SwitchValue(newValue);
        m_passNbr = 0;
        m_valueIsChanging = true;
        if (m_needToFadeIn)
            CheckToStartChangeScaleCoroutine(m_maxSizeAnim, m_speedToFadeIn, m_distanceToMaxSize);
        else
            CheckToStartChangeScaleCoroutine(m_minSizeAnim, m_speedToFadeOff, m_distanceToMinSize);
    }

    void CheckToStartChangeScaleCoroutine(Vector3 newScale, float speed, float distance)
    {
        if (m_currentChangementValues != null)
            StopCoroutine(m_currentChangementValues);

        m_currentChangementValues = ChangeScale(newScale, speed, distance);
        StartCoroutine(m_currentChangementValues);
    }

    IEnumerator ChangeScale(Vector3 newScale, float speed, float distance)
    {
        m_passNbr ++;

        Vector3 fromValue = m_rectTrans.localScale;
        Vector3 toValue = newScale;

        Vector3 actualValue = fromValue;
        float fracJourney = 0;

        while (fracJourney < 1)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualValue = Vector3.Lerp(fromValue, toValue, m_curveAnim.Evaluate(fracJourney));
            SetRectTransScale(actualValue);
            yield return null;
        }

        if (m_passNbr == 1)
            StartCoroutine(ChangeScale(m_startScale, speed, distance));
        if (m_passNbr == 2)
            m_valueIsChanging = false;
    }

    void SetRectTransScale(Vector3 newScale)
    {
        m_rectTrans.localScale = newScale;
    }

    public override void StopChangingValues()
    {
        base.StopChangingValues();
    }
    
}
