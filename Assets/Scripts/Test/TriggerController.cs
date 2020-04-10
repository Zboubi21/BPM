using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    ParcoursController controller;


    public void GiveController(ParcoursController ctrl)
    {
        controller = ctrl;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.OnTriggerHasBeenEntered(gameObject.GetComponent<Collider>());
        }
    }

}
