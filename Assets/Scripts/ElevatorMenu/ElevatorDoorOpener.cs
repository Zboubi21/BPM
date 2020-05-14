using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpener : MonoBehaviour
{
    Animator animator;
    //public Canvas canvas;

    public bool usePlayerStart = false;



    private void Start()
    {
        animator = GetComponent<Animator>();
        if (usePlayerStart)
        {
            if (PlayerController.s_instance != null)
            {
                //canvas.gameObject.SetActive(true);
                PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
                Cursor.visible = true;
            }
        }
    }


    public void OpenDoors()
    {
        animator.SetTrigger("Open");
        PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(true);
    }
}
