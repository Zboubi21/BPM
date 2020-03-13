using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChangerLauncher : MonoBehaviour
{
    
    [SerializeField] bool m_launchAtStart = true;
    [SerializeField] Launcher[] m_launcher;

    [System.Serializable] class Launcher
    {
        public ValueChanger m_valueChanger;
        public float m_waitTimeToLaunch;
        [HideInInspector] public bool m_valueChangerIsLaunched = false;
    }

    bool m_isLaunching = false;
    float m_timer = 0;

    void Start()
    {
        if(m_launchAtStart)
        {
            StartLauncher();
        }
    }

    void Update()
    {
        if(m_isLaunching)
        {
            for (int i = 0, l = m_launcher.Length; i < l; ++i)
            {
                if(m_timer >= m_launcher[i].m_waitTimeToLaunch && !m_launcher[i].m_valueChangerIsLaunched)
                {
                    m_launcher[i].m_valueChangerIsLaunched = true;
                    m_launcher[i].m_valueChanger.On_StartValueChanger();
                }
            }
            m_timer += Time.deltaTime;
        }
    }

    public void StartLauncher()
    {
        m_isLaunching = true;
    }

}
