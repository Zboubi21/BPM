using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBpmCrosshairController : MonoBehaviour
{
    
    [Header("Crosshairs")]
    [SerializeField] PlayerBpmCrosshair[] m_crosshairs = new PlayerBpmCrosshair[4];

    [Header("Colors")]
    [SerializeField] Color m_baseColor = Color.white;
    [SerializeField] Color m_onEnemyNoSpot = Color.red;
    [SerializeField] Color m_onEnemyWeakspot = Color.yellow;

    [Header("Size")]
    [SerializeField] float m_additionalSizePerShoot = 0.1f;
    [SerializeField] int m_maxShoot = 5;

    [Header("Anim")]
    [SerializeField] float m_waitTimeToResetPosWhenNoShoot = 0.5f;
    [SerializeField] float m_speedToResetPos = 5;
    [SerializeField] AnimationCurve m_resetPosCurve;

    bool m_checkToResetPos = false;
    float m_currentTimerToResetPos = 0;
    int m_currentShootNbr = 0;

    void Start()
    {   
        Setup();
    }
    void FixedUpdate()
    {
        if (m_checkToResetPos)
            m_currentTimerToResetPos += Time.deltaTime;
        if (m_currentTimerToResetPos > m_waitTimeToResetPosWhenNoShoot)
        {
            On_ResetPosition();
        }
    }
    void Setup()
    {
        for (int i = 0, l = m_crosshairs.Length; i < l; ++i)
        {
            if (m_crosshairs[i] == null)
                return;
            m_crosshairs[i].AdditionalSizePerShoot = m_additionalSizePerShoot;
            m_crosshairs[i].MaxShoot = m_maxShoot;
        }
    }
    public void On_ChangeCrosshairColor(string tag)
    {
        if (tag == "NoSpot")
        {
            SetImagesColor(m_crosshairs, m_onEnemyNoSpot);
        }
        else if (tag == "WeakSpot")
        {
            SetImagesColor(m_crosshairs, m_onEnemyWeakspot);
        }
        else
        {
            SetImagesColor(m_crosshairs, m_baseColor);
        }
    }
    void SetImagesColor(PlayerBpmCrosshair[] crosshair, Color color)
    {
        for (int i = 0, l = crosshair.Length; i < l; ++i)
        {
            if (crosshair[i] != null)
                if (crosshair[i].CrosshairImage.color != color)
                    crosshair[i].CrosshairImage.color = color;
        }
    }
    public void On_Shoot()
    {
        m_currentShootNbr ++;
        if (m_currentShootNbr > m_maxShoot)
            return;
        
        for (int i = 0, l = m_crosshairs.Length; i < l; ++i)
        {
            if (m_crosshairs[i] == null)
                return;
            m_crosshairs[i].On_Shoot();
        }
    }
    public void On_StopShoot()
    {
        // Debug.Log("On_StopShoot");
        m_currentTimerToResetPos = 0;
        m_checkToResetPos = true;
    }
    void On_ResetPosition()
    {
        for (int i = 0, l = m_crosshairs.Length; i < l; ++i)
        {
            if (m_crosshairs[i] == null)
                return;
            m_crosshairs[i].On_ResetPosition(true, m_speedToResetPos, m_resetPosCurve);
        }
        m_currentTimerToResetPos = 0;
        m_checkToResetPos = false;
        m_currentShootNbr = 0;
    }
    
}
