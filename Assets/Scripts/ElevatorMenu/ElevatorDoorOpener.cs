using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpener : MonoBehaviour
{
    Animator animator;

    public bool usePlayerStart = false;
    static public bool hasBeenInMenu = false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        if (usePlayerStart && !hasBeenInMenu)
        {
            if (PlayerController.s_instance != null)
            {
                //canvas.gameObject.SetActive(true);
                PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
                //Cursor.visible = true;
            }
        }
        else
        {
            PlayerController.s_instance.transform.position = GameManager.Instance.RespawnPos.position;
        }
    }

    public void ButtonStart()
    {
        animator.SetTrigger("Open");
        hasBeenInMenu = true;
        PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(true);
    }

    public void OnLeavingElevator()
    {
        animator.SetTrigger("Close");
    }

    public void OnQuittingToMenu()
    {
        hasBeenInMenu = false;
        SceneReloader.s_instance?.On_ResetLvl();
    }
}
