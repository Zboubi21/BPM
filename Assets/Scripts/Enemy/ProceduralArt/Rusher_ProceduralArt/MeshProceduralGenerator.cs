using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MeshProceduralGenerator : MonoBehaviour
{
    public AllPossibleMeshs allPossibleMeshs;
    public bool useShowCase;
    int maxArmsIndex;
    int maxTorsoIndex;
    int maxBottomHeadIndex;
    int maxTopHeadIndex;

    private void Start()
    {
        for (int i = 0, l = allPossibleMeshs.allArmsArray.Length; i < l; ++i)
        {
            //allPossibleMeshs.allArmsArray[i].gameObject.SetActive(false);
            maxArmsIndex++;
        }
        for (int i = 0, l = allPossibleMeshs.allTorsoArray.Length; i < l; ++i)
        {
            //allPossibleMeshs.allTorsoArray[i].gameObject.SetActive(false);
            maxTorsoIndex++;
        }
        for (int i = 0, l = allPossibleMeshs.allBottomHeadArray.Length; i < l; ++i)
        {
           // allPossibleMeshs.allBottomHeadArray[i].gameObject.SetActive(false);
            maxBottomHeadIndex++;
        }
        for (int i = 0, l = allPossibleMeshs.allTopHeadArray.Length; i < l; ++i)
        {
            //allPossibleMeshs.allTopHeadArray[i].gameObject.SetActive(false);
            maxTopHeadIndex++;
        }

        if (useShowCase)
        {
            StartCoroutine(ShowCaseGeneration());
        }
    }

    IEnumerator ShowCaseGeneration()
    {
        while (useShowCase)
        {
            yield return new WaitForSeconds(1f);
            BuildCharaMesh();
            StartCoroutine(ShowCaseGeneration());
        }
    }


    public void BuildCharaMesh()
    {
        for (int i = 0, l = allPossibleMeshs.allArmsArray.Length; i < l; ++i)
        {
            allPossibleMeshs.allArmsArray[i].gameObject.SetActive(false);
        }
        for (int i = 0, l = allPossibleMeshs.allTorsoArray.Length; i < l; ++i)
        {
            allPossibleMeshs.allTorsoArray[i].gameObject.SetActive(false);
        }
        for (int i = 0, l = allPossibleMeshs.allBottomHeadArray.Length; i < l; ++i)
        {
            allPossibleMeshs.allBottomHeadArray[i].gameObject.SetActive(false);
        }
        for (int i = 0, l = allPossibleMeshs.allTopHeadArray.Length; i < l; ++i)
        {
            allPossibleMeshs.allTopHeadArray[i].gameObject.SetActive(false);
        }

        int chosenArmIndex = UnityEngine.Random.Range(0, maxArmsIndex);
        int chosenTorsoIndex = UnityEngine.Random.Range(0, maxTorsoIndex);
        int chosenBottomHeadIndex = UnityEngine.Random.Range(0, maxBottomHeadIndex);
        int chosenTopHeadIndex = UnityEngine.Random.Range(0, maxTopHeadIndex);

        allPossibleMeshs.allArmsArray[chosenArmIndex].gameObject.SetActive(true);
        allPossibleMeshs.allTorsoArray[chosenTorsoIndex].gameObject.SetActive(true);
        allPossibleMeshs.allBottomHeadArray[chosenBottomHeadIndex].gameObject.SetActive(true);
        allPossibleMeshs.allTopHeadArray[chosenTopHeadIndex].gameObject.SetActive(true);
    }

}
[Serializable] public class AllPossibleMeshs
{
    public GameObject[] allArmsArray;

    public GameObject[] allTorsoArray;

    public GameObject[] allBottomHeadArray;

    public GameObject[] allTopHeadArray;
}
