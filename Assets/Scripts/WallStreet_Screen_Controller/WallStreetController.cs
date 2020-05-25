using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class WallStreetController : MonoBehaviour
{

    public bool displayLoreRandomly;
    [Space]
    public LoreText[] allPossibleLoreText;
    public TMP_Text nextWaveText;
    
    int currentArrayIndex;
    private void Start()
    {
        if (!CheckForAnOverride())
        {
            if (displayLoreRandomly)
            {
                int randomIndex = UnityEngine.Random.Range(0, allPossibleLoreText.Length);
                allPossibleLoreText[randomIndex].loreText.gameObject.SetActive(true);
                currentArrayIndex = randomIndex;
            }
            else
            {
                allPossibleLoreText[0].loreText.gameObject.SetActive(true);
                currentArrayIndex = 0;
            }
        }
        StartCoroutine(SwitchLoreTextDisplayed(allPossibleLoreText[currentArrayIndex].displayTime));
    }
    bool CheckForAnOverride()
    {
        for (int i = 0, l = allPossibleLoreText.Length; i < l; ++i)
        {
            if (allPossibleLoreText[i].chooseThisTextAsFirstText)
            {
                allPossibleLoreText[i].loreText.gameObject.SetActive(true);
                currentArrayIndex = i;
                return true;
            }
        }
        return false;
    }

    IEnumerator SwitchLoreTextDisplayed(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        allPossibleLoreText[currentArrayIndex].loreText.gameObject.SetActive(false);
        if (displayLoreRandomly)
        {
            int randomIndex = ChooseNewIndex(currentArrayIndex);
            currentArrayIndex = randomIndex;
            allPossibleLoreText[currentArrayIndex].loreText.gameObject.SetActive(true);
            StartCoroutine(SwitchLoreTextDisplayed(allPossibleLoreText[randomIndex].displayTime));
        }
        else
        {
            currentArrayIndex++;
            if(currentArrayIndex >= allPossibleLoreText.Length)
            {
                currentArrayIndex = 0;

            }
            allPossibleLoreText[currentArrayIndex].loreText.gameObject.SetActive(true);
            StartCoroutine(SwitchLoreTextDisplayed(allPossibleLoreText[currentArrayIndex].displayTime));
        }
    }
    int ChooseNewIndex(int currentIndex)
    {
        int newIndex = currentIndex;
        while (newIndex == currentIndex)
        {
            newIndex = UnityEngine.Random.Range(0, allPossibleLoreText.Length);
        }
        return newIndex;
    }


    public void ChangeToNextWaveText()
    {
        StopAllCoroutines();
        allPossibleLoreText[currentArrayIndex].loreText.gameObject.SetActive(false);
        nextWaveText.gameObject.SetActive(true);
    }

    public void ChangeToLoreText()
    {
        StopAllCoroutines();
        nextWaveText.gameObject.SetActive(false);
        StartCoroutine(SwitchLoreTextDisplayed(0));
    }

}

[Serializable] public class LoreText
{
    public bool chooseThisTextAsFirstText;
    public TMP_Text loreText;
    public float displayTime;
}
