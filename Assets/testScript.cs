using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform from;
    public Transform to;

    private float timeCount = 0.0f;

    //void Update()
    //{
    //    transform.position = 
    //    transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, timeCount);
    //    
    //}
    public float time;
    float _currentTime;
    public Transform target;
    bool go;
    Vector3 relativePos;
    Vector3 initForward;
    void Update()
    {

        if ((Input.GetKeyDown(KeyCode.Alpha8) || go) && timeCount <= 1)
        {
            go = true;
            LetsGo();
        }
        else
        {
            go = false;
            timeCount = 0;
            _currentTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            relativePos = target.position - transform.position;
            initForward = transform.forward;
        }

    }

    void LetsGo()
    { 
        _currentTime += Time.deltaTime;
        timeCount = Mathf.InverseLerp(0, time, _currentTime);
        Quaternion rotation = Quaternion.LookRotation(Vector3.Slerp(initForward, relativePos, timeCount));

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotation.eulerAngles.y, transform.eulerAngles.z);
    }
}
