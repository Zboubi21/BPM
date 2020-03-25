using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageValues : MonoBehaviour
{
    
    [SerializeField] StartType m_startValue = StartType.StartWithFromValue;
    [SerializeField] float m_timeToFadeIn = 0.25f, m_timeToFadeOff = 0.5f;
    [SerializeField] Color m_fromColor, m_toColor;

    enum StartType
    {
        StartWithFromValue,
        StartWithToValue,
    }

    Image m_targetImage;
    bool m_needToFadeIn = true; // Si m_needToFadeIn == false alors c'est qu'il faut FadeOut
    IEnumerator m_currentChangementValues;
    float m_distanceFromTargetedColors;
    float m_speedToFadeIn, m_speedToFadeOff;

    void Start()
    {
        m_targetImage = GetComponent<Image>();

        if (m_startValue == StartType.StartWithFromValue)
        {
            SetImageColor(m_fromColor);
            m_needToFadeIn = true;
        }
        else
        {
            SetImageColor(m_toColor);
            m_needToFadeIn = false;
        }

        m_distanceFromTargetedColors = GetDistanceFromColors(m_fromColor, m_toColor);
        // vitesse = distance / temps
        m_speedToFadeIn = m_distanceFromTargetedColors / m_timeToFadeIn;
        m_speedToFadeOff = m_distanceFromTargetedColors / m_timeToFadeOff;
    }

    public void SwitchImageValue()
    {
        if (m_needToFadeIn)
        {
            CheckToStartChangeImageColorCoroutine(m_toColor, m_timeToFadeIn);
        }
        else
        {
            CheckToStartChangeImageColorCoroutine(m_fromColor, m_timeToFadeOff);
        }
    }
    float GetTimeToDoValue(bool needToFadeIn)
    {
        if (needToFadeIn)
        {

        }
        else
        {

        }
        return 0;
    }

    void CheckToStartChangeImageColorCoroutine(Color toColor, float timeToDo)
    {
        if (m_currentChangementValues != null)
            StopCoroutine(m_currentChangementValues);

        m_currentChangementValues = ChangeImageColor(toColor, timeToDo);
        StartCoroutine(m_currentChangementValues);
    }
    IEnumerator ChangeImageColor(/*Color fromColor, */Color toColor, float timeToDo)
    {
        // Color fromColor = m_fromColor;
        // Color toColor = m_toColor;
        // float timeToDo = m_timeToFadeIn;

        Color fromColor = m_targetImage.color;

        Color actualColor = fromColor;
        float fracJourney = 0;
        float distance = m_distanceFromTargetedColors;
        float speed = distance / timeToDo;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualColor = Color.Lerp(fromColor, toColor, fracJourney);
            SetImageColor(actualColor);
            yield return null;
        }
    }
    float GetDistanceFromColors(Color color1, Color color2)
    {
        return Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b) + Mathf.Abs(color1.a - color2.a);
    }
    void SetImageColor(Color newColor)
    {
        if (m_targetImage.color != newColor)
            m_targetImage.color = newColor;
    }
    
}
