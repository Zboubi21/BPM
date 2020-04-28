using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            ParticleSystem[] VFXArray = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem VFX in VFXArray)
            {
                VFX.Play();
            }
        }
    }
}
