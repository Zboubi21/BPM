using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChangerTrigger : MonoBehaviour
{
    
    [SerializeField] MovableArea[] m_movableArea;

    [System.Serializable] class MovableArea
    {
        public KeyCode m_activationInput;
        public ValueChanger[] m_movableObjects;
        [HideInInspector] public int[] m_currentValue;
    }

    void Start()
    {
        for (int i = 0, l = m_movableArea.Length; i < l; ++i)
        {
            m_movableArea[i].m_currentValue = new int[m_movableArea[i].m_movableObjects.Length];
            for (int i2 = 0, l2 = m_movableArea[i].m_currentValue.Length; i2 < l2; ++i2)
            {
                m_movableArea[i].m_currentValue[i2] = 1;
            }
        }
    }

    void Update()
    {
        for (int i = 0, l = m_movableArea.Length; i < l; ++i)
        {
            if (Input.GetKeyDown(m_movableArea[i].m_activationInput))
            {
                for (int i2 = 0, l2 = m_movableArea[i].m_movableObjects.Length; i2 < l2; ++i2)
                {
                    if (m_movableArea[i].m_movableObjects[i2].GetValueChangerLength() <= m_movableArea[i].m_currentValue[i2])
                    {
                        m_movableArea[i].m_currentValue[i2] = 0;
                    }
                    m_movableArea[i].m_movableObjects[i2].On_StartValueChanger(m_movableArea[i].m_currentValue[i2]);
                    m_movableArea[i].m_currentValue[i2] ++;
                }
            }
        }
    }
    
}
