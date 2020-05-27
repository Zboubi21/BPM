using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologrammeShaderController : MonoBehaviour
{
    
    [SerializeField] string m_parameters;
    [SerializeField] float m_targetValue = 0;
    [SerializeField] float m_timeToAnim = 0.5f;
    [SerializeField] AnimationCurve m_animCurve;
    [SerializeField] Renderer[] m_renderers;

    bool m_done = false;

    public void SwitchValue()
    {
        if (m_done)
            return;
        m_done = true;

        CustomAnimationManager.AnimFloatWithTime(m_renderers[0].material.GetFloat(m_parameters), m_targetValue, m_timeToAnim).SetCurve(m_animCurve).SetOnUpdate(ChangeShaderValue);
    }
    
    void ChangeShaderValue(float newvalue)
    {
        for (int i = 0, l = m_renderers.Length; i < l; ++i)
        {
            m_renderers[i].material.SetFloat(m_parameters, newvalue);
        }
    }

}
