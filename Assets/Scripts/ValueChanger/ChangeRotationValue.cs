using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRotationValue : ChangeValues
{
    
    [SerializeField] Vector3 m_toRotation = new Vector3(0, -125, 0);
    [SerializeField] float m_timeToDoAnim = 0.2f;
    [SerializeField] AnimationCurve m_curveAnim = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(0, 0));

    Quaternion m_fromRot;
    Quaternion m_toRot;
    float m_animSpeed;
    float m_distanceToAnim;

    void Awake()
    {
        m_fromRot = transform.localRotation;
        m_toRot = Quaternion.Euler(m_toRotation);
        m_distanceToAnim = GetDistanceFromQuaternion(m_fromRot, m_toRot);
        m_animSpeed = m_distanceToAnim / m_timeToDoAnim;
    }
    protected override void SetupChangeValue(bool startWithFromValue)
    {
        base.SetupChangeValue(startWithFromValue);
    }

    public override void SwitchValue()
    {
        base.SwitchValue();
        CheckToStartChangeScaleCoroutine();
    }
    void CheckToStartChangeScaleCoroutine()
    {
        StopAllCoroutines();
        m_currentChangementValues = ChangeRotation();
        StartCoroutine(m_currentChangementValues);
    }

    IEnumerator ChangeRotation()
    {
        Quaternion currentRot = m_fromRot;
        float fracJourney = 0;

        while (fracJourney < 1)
        // while (actualValue != m_toRot)
        {
            fracJourney += (Time.deltaTime) * m_animSpeed / m_distanceToAnim;
            currentRot = Quaternion.Slerp(m_fromRot, m_toRot, m_curveAnim.Evaluate(fracJourney));
            transform.localRotation = currentRot;
            yield return null;
        }
    }
    
}
