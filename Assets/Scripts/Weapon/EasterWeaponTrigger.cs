using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class EasterWeaponTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<EasterWeaponEnable>().OnActivate();
            ObjectPooler.Instance.ReturnObjectToPool(ObjectType.Gun, gameObject);
        }
    }
}
