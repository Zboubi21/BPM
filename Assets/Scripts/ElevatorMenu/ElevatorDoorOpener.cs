using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpener : MonoBehaviour
{
    Animator animator;
    PlayerController controller;
    public Canvas canvas;

    public void Start()
    {
        controller = PlayerController.s_instance;
    }

    public void OpenDoors()
    {
        animator.SetTrigger("Open");
    }

    private void Update()
    {
        if(controller != null)
        {
            if (canvas.gameObject.activeSelf && Cursor.visible == false)
            {
                canvas.gameObject.SetActive(false);
            }
            else if (!canvas.gameObject.activeSelf && Cursor.visible == true)
            {
                canvas.gameObject.SetActive(true);
            }
        }
    }
}
