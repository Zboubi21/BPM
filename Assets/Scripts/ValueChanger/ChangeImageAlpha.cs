using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageAlpha : MonoBehaviour
{
    
    [Header("Anims")]
    [SerializeField] Parameters m_fadeIn, m_fadeOut;
    [System.Serializable] class Parameters
    {
        public float m_targetValue = 0;
        public float m_animSpeed = 50;
        public AnimationCurve m_animCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    }

    [Header("Values")]
    [SerializeField] int m_additionnelAlphaPerHit = 10;
    [SerializeField] int m_maxHitNbr = 5;
    [SerializeField] float m_waitTimeToResetAlpha = 1.5f;
    [SerializeField] float m_waitTimeToResetAlphaAfterCriticalLvlOfBpm = 0.75f;

    [Header("Blink")]
    [SerializeField] Parameters m_baseBlink;
    [SerializeField] Parameters m_minBlink, m_maxBlink;
    [SerializeField] float m_waitTimeBetweenBlink = 0.4f;

    Image m_image;
    bool m_isInCriticalLevelOfBpm = false;
    AnimationData m_animData;
    
    int m_currentHitNbr = 0;

    void Start()
    {
        m_image = GetComponent<Image>();
    }

    public void On_TakeDamage()
    {
        // Debug.Log("TakeDamage");
        if (!m_isInCriticalLevelOfBpm)
        {
            StartResetAlphaCorout(m_waitTimeToResetAlpha);
            CustomAnimationManager.StopAnimation(m_animData);
            m_animData = CustomAnimationManager.AnimFloatWithSpeed(GetCurrentAlpha(), GetTargetedAlpha(), m_fadeIn.m_animSpeed).SetCurve(m_fadeIn.m_animCurve).SetOnUpdate(SetAlpha);
        }
        else
        {
            StartResetAlphaCorout(0, true);
            CustomAnimationManager.StopAnimation(m_animData);
            m_isBlinkOn = true;
            m_animData = CustomAnimationManager.AnimFloatWithSpeed(GetCurrentAlpha(), m_baseBlink.m_targetValue, m_baseBlink.m_animSpeed).SetCurve(m_baseBlink.m_animCurve).SetOnUpdate(SetAlpha).SetOnComplete(BlinkAlpha);
        }
        if (m_currentHitNbr < m_maxHitNbr)
            m_currentHitNbr ++;
    }
    public void On_EnterInCriticalLevelOfBpm(bool inCriticalLevelOfBpm)
    {
        // Debug.Log("On_EnterInCriticalLevelOfBpm = " + inCriticalLevelOfBpm);
        m_isInCriticalLevelOfBpm = inCriticalLevelOfBpm;
        if (!m_isInCriticalLevelOfBpm)
            StartResetAlphaCorout(m_waitTimeToResetAlphaAfterCriticalLvlOfBpm);
            // ResetImageAlpha();
    }

    bool m_isBlinkOn = true;
    void BlinkAlpha()
    {
        if (!m_isInCriticalLevelOfBpm)
            return;
        m_isBlinkOn =! m_isBlinkOn;
        CustomAnimationManager.StopAnimation(m_animData);
        if (m_isBlinkOn)
            m_animData = CustomAnimationManager.AnimFloatWithSpeed(GetCurrentAlpha(), m_maxBlink.m_targetValue, m_maxBlink.m_animSpeed).SetCurve(m_maxBlink.m_animCurve).SetOnUpdate(SetAlpha).SetOnComplete(BlinkAlpha).SetDelay(m_waitTimeBetweenBlink);
        else
            m_animData = CustomAnimationManager.AnimFloatWithSpeed(GetCurrentAlpha(), m_minBlink.m_targetValue, m_minBlink.m_animSpeed).SetCurve(m_minBlink.m_animCurve).SetOnUpdate(SetAlpha).SetOnComplete(BlinkAlpha);
    }

    float GetTargetedAlpha()
    {
        return m_fadeIn.m_targetValue + m_additionnelAlphaPerHit * m_currentHitNbr;
    }

    void StartResetAlphaCorout(float waitTime, bool justResetCorout = false)
    {
        if (m_waitTimeToResetAlphaCorout != null)
            StopCoroutine(m_waitTimeToResetAlphaCorout);
        if (!justResetCorout)
            m_waitTimeToResetAlphaCorout = StartCoroutine(WaitTimeToResetAlpha(waitTime));
    }
    Coroutine m_waitTimeToResetAlphaCorout;
    IEnumerator WaitTimeToResetAlpha(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ResetImageAlpha();
    }
    void ResetImageAlpha()
    {
        m_currentHitNbr = 0;
        CustomAnimationManager.StopAnimation(m_animData);
        m_animData = CustomAnimationManager.AnimFloatWithSpeed(GetCurrentAlpha(), m_fadeOut.m_targetValue, m_fadeOut.m_animSpeed).SetCurve(m_fadeOut.m_animCurve).SetOnUpdate(SetAlpha);
    }

    float GetCurrentAlpha()
    {
        return m_image.color.a * 255;
    }
    void SetAlpha(float newAlpha)
    {
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, newAlpha / 255);
    }
    
}
