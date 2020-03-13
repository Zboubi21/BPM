using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    Renderer rend;
    public Shader shader;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Use the Specular shader on the material
        rend.material.shader = shader;
    }

    void Update()
    {
        // Animate the Shininess value
        float chromapropability = Mathf.PingPong(Time.time, 1.0f);

        Debug.Log(rend.material.GetFloat("chromapropability"));

        rend.material.SetFloat("chromapropability", chromapropability);
    }
}
