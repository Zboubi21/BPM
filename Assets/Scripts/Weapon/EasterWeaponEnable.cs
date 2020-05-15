using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterWeaponEnable : MonoBehaviour
{
    public GameObject[] _allObjectToActivate;
    public GameObject[] _allObjectToDeactivate;

    public void OnActivate()
    {
        for (int i = 0, l = _allObjectToActivate.Length; i < l; ++i)
        {
            _allObjectToActivate[i].SetActive(true);
        }
        for (int i = 0, l = _allObjectToDeactivate.Length; i < l; ++i)
        {
            _allObjectToDeactivate[i].SetActive(false);
        }
    }
}
