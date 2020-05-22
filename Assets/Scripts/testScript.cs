using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform from;
    public Transform to;

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(from.localRotation, to.localRotation, Time.time);
        transform.position = Vector3.Lerp(from.localPosition, to.localPosition, Time.time);
    }
}
