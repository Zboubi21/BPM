using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShaderValue_TvPub : ChangeShaderValue
{
    
    [Header("TV PUB")]
    [SerializeField] float m_startDelay = 1;
    [SerializeField] bool m_stopChangeValueWhenIsDestroyed = true;
    [SerializeField] string m_pubShaderParameter;
    [SerializeField] Parameters m_pubFromValue;
    [SerializeField] Parameters m_pubToValue;

    [System.Serializable] class Parameters
    {
        [Header("Delay")]
        public float m_minTimeToStart = 5;
        public float m_maxTimeToStart = 10;

        [Header("Parameters")]
        public float m_targetValue = 0;
        public float m_timeToAnim = 1;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    }

    bool m_needToFadeInPub = true;
    bool m_canChangePubShaderValue = true;
    AnimationData m_pubAnim;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(WaitTimeToStart());
    }
    IEnumerator WaitTimeToStart()
    {
        yield return new WaitForSeconds(m_startDelay);
        On_AnimTvPub();
    }
    void On_AnimTvPub()
    {
        if (!m_canChangePubShaderValue)
            return;
        CustomAnimationManager.StopAnimation(m_pubAnim);
        float waitAlea = m_needToFadeInPub ? Random.Range(m_pubToValue.m_minTimeToStart, m_pubToValue.m_maxTimeToStart) : Random.Range(m_pubFromValue.m_minTimeToStart, m_pubFromValue.m_maxTimeToStart);
        if (m_needToFadeInPub)
            m_pubAnim = CustomAnimationManager.AnimFloatWithTime(GetShaderValue(m_pubShaderParameter), m_pubToValue.m_targetValue, m_pubToValue.m_timeToAnim).SetCurve(m_pubToValue.m_curve).SetDelay(waitAlea).SetOnUpdate(SetPubShaderValue).SetOnComplete(On_AnimTvPub);
        else
            m_pubAnim = CustomAnimationManager.AnimFloatWithTime(GetShaderValue(m_pubShaderParameter), m_pubFromValue.m_targetValue, m_pubFromValue.m_timeToAnim).SetCurve(m_pubFromValue.m_curve).SetDelay(waitAlea).SetOnUpdate(SetPubShaderValue).SetOnComplete(On_AnimTvPub);
        m_needToFadeInPub =! m_needToFadeInPub;
    }

    void SetPubShaderValue(float newValue)
    {
        SetShaderValue(m_pubShaderParameter, newValue);
    }

    public void StopChangeValueWhenIsDestroyed()
    {
        if (!m_stopChangeValueWhenIsDestroyed)
            return;
        CustomAnimationManager.StopAnimation(m_pubAnim);
        m_canChangePubShaderValue = false;
    }

}
