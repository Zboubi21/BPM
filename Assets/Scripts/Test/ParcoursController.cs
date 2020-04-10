using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParcoursController : MonoBehaviour
{
    public Collider startOfParcour;
    public Collider endOfParcour;

    public TMP_Text[] minutesAndSecondsText;
    public TMP_Text[] microSecondsText;


    float passingTime;
    int minutes;
    int secondes;
    int microSeconds;

    bool crossStarts;

    private void Awake()
    {
        startOfParcour.gameObject.GetComponent<TriggerController>().GiveController(this);
        endOfParcour.gameObject.GetComponent<TriggerController>().GiveController(this);
        passingTime = 1;
    }


    private void Update()
    {
        if (crossStarts)
        {
            passingTime += Time.deltaTime;
            minutes = Mathf.FloorToInt(passingTime / 60);
            secondes = Mathf.FloorToInt((passingTime % 60));

            if (minutes < 10)
            {
                if(secondes < 10)
                {
                    for (int i = 0, l = minutesAndSecondsText.Length; i < l; i++)
                    {
                        minutesAndSecondsText[i].text = string.Format("0{0} : 0{1}", minutes, secondes);
                    }
                }
                else
                {
                    for (int i = 0, l = minutesAndSecondsText.Length; i < l; i++)
                    {
                        minutesAndSecondsText[i].text = string.Format("0{0} : {1}", minutes, secondes);
                    }
                }
            }
            else
            {
                if (secondes < 10)
                {
                    for (int i = 0, l = minutesAndSecondsText.Length; i < l; i++)
                    {
                        minutesAndSecondsText[i].text = string.Format("{0} : 0{1}", minutes, secondes);
                    }
                }
                else
                {
                    for (int i = 0, l = minutesAndSecondsText.Length; i < l; i++)
                    {
                        minutesAndSecondsText[i].text = string.Format("{0} : {1}", minutes, secondes);
                    }
                }
            }

            if(microSeconds < 10)
            {
                for (int i = 0, l = microSecondsText.Length; i < l; i++)
                {
                    microSecondsText[i].text = string.Format("00{0}", microSeconds);
                }
            }
            else if (microSeconds < 100)
            {
                for (int i = 0, l = microSecondsText.Length; i < l; i++)
                {
                    microSecondsText[i].text = string.Format("0{0}", microSeconds);
                }
            }
            else
            {
                for (int i = 0, l = microSecondsText.Length; i < l; i++)
                {
                    microSecondsText[i].text = string.Format("{0}", microSeconds);
                }
            }

            if (microSeconds >= 999)
            {
                microSeconds = 0;
            }

            microSeconds = Mathf.FloorToInt(passingTime * 1000)%1000;
        }
    }


    public void OnTriggerHasBeenEntered(Collider crossedCollider)
    {
        if(crossedCollider == startOfParcour)
        {
            crossStarts = true;
        }
        else if(crossedCollider == endOfParcour)
        {
            crossStarts = false;
        }
    }
}
