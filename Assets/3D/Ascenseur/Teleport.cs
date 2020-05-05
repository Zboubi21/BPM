using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public Transform target = null; //Teleport1
    public Transform target2 = null; //Teleport2
    bool bJump = false; // is teleport 1 active ? 
    bool bJump2 = false; //is teleport 2 active ? 
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Teleport" && bJump==false && bJump2==false ) // jump from teleport 1 to teleport 2 
        {
            this.transform.position = target.position;
            bJump = true;
        }

        if (other.gameObject.tag == "Teleport2" && bJump==false && bJump2==false ) // jump from teleport 2 to teleport 1
        {
            this.transform.position = target2.position;
            bJump2 = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Teleport")
        {
            bJump2 = false; // Deactivate teleport 2
        }
        if (other.gameObject.tag == "Teleport2")
        {
            bJump = false; // deactive teleport 1
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
