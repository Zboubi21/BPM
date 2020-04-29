using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectFollowTarget : MonoBehaviour
{
    float posY;
    private void Start()
    {
        posY = PlayerController.s_instance.GetComponent<CapsuleCollider>().center.y;
    }
    private void Update()
    {
        transform.position = new Vector3(PlayerController.s_instance.transform.position.x, PlayerController.s_instance.transform.position.y + posY, PlayerController.s_instance.transform.position.z);
    }
}
