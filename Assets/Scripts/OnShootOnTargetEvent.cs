using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnShootOnTargetEvent : MonoBehaviour
{

    RaycastHit _hit;
    Camera playerCamera;
    LayerMask rayCastCollision;

    public UnityEvent OnFirstShootEvent;
    public UnityEvent OnSecondShootEvent;

    private void Start()
    {
        playerCamera = PlayerController.s_instance.GetComponent<WeaponPlayerBehaviour>().playerCamera;
        rayCastCollision = PlayerController.s_instance.GetComponent<WeaponPlayerBehaviour>().rayCastCollision;
    }

    bool hasBeenShooted;

    void Update()
    {
        if(playerCamera != null)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision))
                {
                    if (_hit.collider.CompareTag("Screen") && _hit.collider == gameObject.GetComponent<Collider>())
                    {
                        if (!hasBeenShooted)
                        {
                            hasBeenShooted = true;
                            OnFirstShootEvent.Invoke();
                        }
                        else
                        {
                            hasBeenShooted = false;
                            OnSecondShootEvent.Invoke();
                        }
                    }
                }
            }
        }
    }
}
