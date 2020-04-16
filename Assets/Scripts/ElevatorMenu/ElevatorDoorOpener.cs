using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpener : MonoBehaviour
{
    Animator animator;

    public void Start()
    {
        
    }

    public void OpenDoors()
    {
        animator.SetTrigger("Open");
    }
}
