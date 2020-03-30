using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeShaderValue : MonoBehaviour
{

    // [SerializeField] float m_minValue, m_maxValue;

    Material sliderShader;
    float m_currentValue;

    [SerializeField, Range(0, 600)] float m_currentBPM = 300;

    void Start()
    {
        sliderShader = GetComponent<MeshRenderer>().material;

        // UpdateShaderWithCurrentBPM(300, 0, 600);
    }

    public void UpdateShaderWithCurrentBPM(float currentBPM, float minBPM, float maxBPM)
    {
        // float newValue = Mathf.InverseLerp(minBPM, maxBPM, currentBPM);
        // Debug.Log("newValue = " + newValue);
        // Debug.Log("currentBPM = " + currentBPM / maxBPM);

        // float length = Mathf.Abs(m_maxValue) + Mathf.Abs(m_minValue);
        // Debug.Log("length = " + length);
        
        // // range1 = [0, 100]
        // // range2 = [-80, 0]
        // float mini1 = 0;
        // float mini2 = -80;
        // float range1 = 100;
        // float range2 =  80;
        // float value1 =  56;
        
        // float v2 = (value1 - mini1) * range2 / range1  + mini2;
        // Debug.Log("v2 = " + v2);
        // float myValue = (currentBPM - minBPM) * 133 / 600  + -3;

        float myValue = Mathf.InverseLerp(minBPM, maxBPM, currentBPM);
        Debug.Log("myValue = " + myValue);
        sliderShader.SetFloat("_Silder_BPM", myValue);
        sliderShader.SetFloat("_Slide_BPM_Arriere", myValue);
    }

    void Update()
    {
        // UpdateShaderWithCurrentBPM(m_currentBPM, 0, 600);
    }
    
}
