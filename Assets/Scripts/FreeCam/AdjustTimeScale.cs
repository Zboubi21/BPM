using UnityEngine;
using System.Collections;
using TMPro;


public class AdjustTimeScale : MonoBehaviour
{
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (Time.timeScale < 1.0F)
            {
                Time.timeScale += 0.1f;
            }
               
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (Time.timeScale >= 0.2F)
            {
                Time.timeScale -= 0.1f;
            }
                
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
    }

    void OnApplicationQuit()
    {
        Time.timeScale = 1.0F;
        Time.fixedDeltaTime = 0.02F;
    }
}