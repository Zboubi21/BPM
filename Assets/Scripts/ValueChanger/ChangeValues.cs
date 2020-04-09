using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeValues : MonoBehaviour
{

    [Header("Fade In / Fade Off")]
    [SerializeField] StartType m_startValue = StartType.StartWithFromValue;
    public StartType StartValue { get => m_startValue; set => m_startValue = value; }

    protected bool m_needToFadeIn = true;   // Si m_needToFadeIn == false alors c'est qu'il faut FadeOut
    protected bool m_valueIsChanging = false;
    protected IEnumerator m_currentChangementValues;


    public enum StartType
    {
        StartWithFromValue,
        StartWithToValue,
    }

    protected virtual void Awake()
    {
        if (m_startValue == StartType.StartWithFromValue)
        {
            m_needToFadeIn = false;
            SetupChangeValue(true);
        }
        else
        {
            m_needToFadeIn = true;
            SetupChangeValue(false);
        }
    }
    protected virtual void SetupChangeValue(bool startWithFromValue)
    {
    }

    public virtual void SwitchValue()
    {
        m_needToFadeIn =! m_needToFadeIn;
    }
    public virtual void SwitchValue(bool newValue)
    {
        m_needToFadeIn = newValue;
    }

    public virtual void StopChangingValues()
    {
        if  (!m_valueIsChanging)
            return;
        SwitchValue();
    }

    protected float GetDistanceFromColors(Color color1, Color color2)
    {
        return Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b) + Mathf.Abs(color1.a - color2.a);
    }
    protected float GetDistanceFromVectors(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }
    protected void SetImageColor(Image image, Color newColor)
    {
        if (image.color != newColor)
            image.color = newColor;
    }
    
}
