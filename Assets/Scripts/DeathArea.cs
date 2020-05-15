using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeathArea : MonoBehaviour
{
    
    [SerializeField] bool m_showGizmos = true;

    BoxCollider boxCol;
    public BoxCollider BoxCol { get => boxCol = GetComponent<BoxCollider>(); }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<BPMSystem>().On_PlayerFallIntoTheVoid(transform);
        }
    }

    void OnDrawGizmos()
    {
        if (m_showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + BoxCol.center, BoxCol.size);
        }
    }

}
