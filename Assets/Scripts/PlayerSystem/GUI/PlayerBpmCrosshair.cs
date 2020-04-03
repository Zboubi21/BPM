using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerBpmCrosshair : MonoBehaviour
{

    [SerializeField] bool m_moveX, m_moveY;
    [SerializeField] bool m_addValue;

    Image m_crosshairImage;
    public Image CrosshairImage { get => m_crosshairImage; }
    float m_additionalSizePerShoot = 0.1f;
    public float AdditionalSizePerShoot { get => m_additionalSizePerShoot; set => m_additionalSizePerShoot = value; }
    int m_maxShoot = 5;
    public int MaxShoot { get => m_maxShoot; set => m_maxShoot = value; }

    Vector3 m_startPos;
    IEnumerator m_resetPosAnim;

    void Awake()
    {
        m_crosshairImage = GetComponent<Image>();
        m_startPos = transform.localPosition;
    }

    public void On_Shoot()
    {
        On_ResetPosition(false);
        float xPos = transform.localPosition.x;
        if (m_moveX)
        {
            if (m_addValue)
                xPos += m_additionalSizePerShoot;
            else
                xPos -= m_additionalSizePerShoot;
        }
        float yPos = transform.localPosition.y;
        if (m_moveY)
        {
            if (m_addValue)
                yPos += m_additionalSizePerShoot;
            else
                yPos -= m_additionalSizePerShoot;
        }
        transform.localPosition = new Vector3(xPos, yPos, transform.localPosition.z);
    }
    public void On_ResetPosition(bool resetPos, float speed = 0, AnimationCurve curve = null)
    {
        if (m_resetPosAnim != null)
            StopCoroutine(m_resetPosAnim);
        
        if (resetPos)
        {
            m_resetPosAnim = ResetPos(speed, curve);
            StartCoroutine(m_resetPosAnim);
        }
    }

    IEnumerator ResetPos(float speed, AnimationCurve curve)
    {
        Vector3 fromValue = transform.localPosition;
        Vector3 toValue = m_startPos;
        Vector3 actualValue = fromValue;
        float fracJourney = 0;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * speed / Vector3.Distance(fromValue, toValue);
            actualValue = Vector3.Lerp(fromValue, toValue, curve.Evaluate(fracJourney));
            transform.localPosition = actualValue;
            yield return null;
        }
    }
    
}
