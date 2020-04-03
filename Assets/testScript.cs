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
    public float levelOfInBetween;
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
            timeCount = 0;
            _currentTime = 0;
            lastFrameTargetPos = target.position;
        }
        
    }
    Vector3 lastFrameTargetPos;
    void LetsGo()
    {
        Quaternion rotation;
        //_currentTime += Time.deltaTime;
        if (target.position == lastFrameTargetPos/*Betweener.IsBetween(target.position.x, target.position.x - levelOfInBetween, target.position.x + levelOfInBetween) && Betweener.IsBetween(target.position.y, target.position.y - levelOfInBetween, target.position.y + levelOfInBetween) && Betweener.IsBetween(target.position.z, target.position.z - levelOfInBetween, target.position.z + levelOfInBetween)*/)
        {
            if(_currentTime < time)
            {
                _currentTime += Time.deltaTime;
            }
            else
            {
                _currentTime = time;
            }
            rotation = Quaternion.LookRotation(Vector3.Slerp(initForward, relativePos, Mathf.InverseLerp(0, time, _currentTime)));

        }
        else
        {
            //timeCount = Mathf.InverseLerp(0, time, Time.deltaTime);
            relativePos = target.position - transform.position;
            initForward = transform.forward;
            rotation = Quaternion.LookRotation(Vector3.Slerp(initForward, relativePos, (1/time)*Time.deltaTime));
            _currentTime = 0;
        }
        lastFrameTargetPos = target.position;
        Debug.DrawRay(transform.position, relativePos, Color.red);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotation.eulerAngles.y, transform.eulerAngles.z);
    }

    
}

public static class Betweener
{
    public static bool IsBetween<T>(this T item, T start, T end)
    {
        return Comparer<T>.Default.Compare(item, start) >= 0
            && Comparer<T>.Default.Compare(item, end) <= 0;
    }
}
