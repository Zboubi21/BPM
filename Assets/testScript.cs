using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform from;


    private void Update()
    {
        from.LookAt(PlayerController.s_instance.transform);
    }
}
