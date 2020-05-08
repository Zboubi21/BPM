using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shader_Trigger : MonoBehaviour
{
    public ParticleSystem VFX;
    public float start_Lerp_Value;
    public float end_Lerp_Value;
    public float Lerp_Duration;
    float lerpValue;
    Renderer renderer;

    public string proterty_Name;

    private float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        lerpValue = start_Lerp_Value;
        renderer = GetComponent<Renderer> ();
    }

    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            t = 0;
            VFX.Play();
        }    

        if (VFX.isPlaying)
        {
            lerpValue = Mathf.Lerp(start_Lerp_Value, end_Lerp_Value, t);
            if (t < 1)
            {
                t += Time.deltaTime / Lerp_Duration;
            }
        }     

        renderer.material.SetFloat(proterty_Name, lerpValue);
    }
}
