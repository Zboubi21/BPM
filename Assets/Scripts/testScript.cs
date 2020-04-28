using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform from;


    private void LateUpdate()
    {
        from.LookAt(new Vector3(PlayerController.s_instance.transform.position.x, PlayerController.s_instance.transform.position.y + 1.5f, PlayerController.s_instance.transform.position.z));
    }
}
