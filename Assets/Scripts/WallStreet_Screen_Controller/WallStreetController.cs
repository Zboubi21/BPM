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
    [Header("Fade For NextWave")]
    public FadeHandeler nextWaveFade;
    
    string nextWave = "Next Wave";
    TMP_Text TMPtext;
    int currentArrayIndex;
    private void Start()
    {
        if(TryGetComponent(out TMP_Text text))
        {
            TMPtext = text;
            if (!CheckForAnOverride())
            {
                if (displayLoreRandomly)
                {
                    int randomIndex = UnityEngine.Random.Range(0, allPossibleLoreText.Length);
                    text.text = allPossibleLoreText[randomIndex].loreText;
                    currentArrayIndex = randomIndex;
                }
                else
                {
                    text.text = allPossibleLoreText[0].loreText;
                    currentArrayIndex = 0;
                }
            }
            StartCoroutine(SwitchLoreTextDisplayed(allPossibleLoreText[currentArrayIndex].displayTime));
        }
    }
    bool CheckForAnOverride()
    {
        for (int i = 0, l = allPossibleLoreText.Length; i < l; ++i)
        {
            if (allPossibleLoreText[i].chooseThisTextAsFirstText)
            {
                TMPtext.text = allPossibleLoreText[i].loreText;
                currentArrayIndex = i;
                return true;
            }
        }
        return false;
    }

    IEnumerator SwitchLoreTextDisplayed(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        if (displayLoreRandomly)
        {
            int randomIndex = ChooseNewIndex(currentArrayIndex);
            currentArrayIndex = randomIndex;
            yield return StartCoroutine(ChangeCurrentTextToNextText(allPossibleLoreText[randomIndex].fadeHandeler.fadeEffect, allPossibleLoreText[randomIndex].fadeHandeler.timeOfFade, TMPtext, allPossibleLoreText[randomIndex].loreText));
            StartCoroutine(SwitchLoreTextDisplayed(allPossibleLoreText[randomIndex].displayTime));
        }
        else
        {
            currentArrayIndex++;
            if(currentArrayIndex >= allPossibleLoreText.Length)
            {
                currentArrayIndex = 0;
            }
            yield return StartCoroutine(ChangeCurrentTextToNextText(allPossibleLoreText[currentArrayIndex].fadeHandeler.fadeEffect, allPossibleLoreText[currentArrayIndex].fadeHandeler.timeOfFade, TMPtext, allPossibleLoreText[currentArrayIndex].loreText));
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


    #region Fade With Curve
    IEnumerator ChangeCurrentTextToNextText(AnimationCurve curve, float timeOfAnimation, TMP_Text text, string newText, bool hasToLoop = false)
    {
        yield return StartCoroutine(FadeInFadeOutControl(curve, timeOfAnimation, text, true));
        text.text = newText;
        yield return StartCoroutine(FadeInFadeOutControl(curve, timeOfAnimation, text));
        if (hasToLoop)
        {
            StartCoroutine(ChangeCurrentTextToNextText(curve, timeOfAnimation, text, newText, hasToLoop));
        }
    }

    IEnumerator FadeInFadeOutControl(AnimationCurve curve, float timeOfAnimation, TMP_Text text, bool inverseCurve = false)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / timeOfAnimation <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = curve.Evaluate(_currentTimeOfAnimation / timeOfAnimation);

            if (!inverseCurve)
            {
                text.alpha = Mathf.Lerp(0, 1, value);
            }
            else
            {
                text.alpha = Mathf.Lerp(1, 0, value);
            }

            yield return null;

        }
        _currentTimeOfAnimation = 0;
    }
    #endregion


    public void ChangeToNextWaveText()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeCurrentTextToNextText(nextWaveFade.fadeEffect, nextWaveFade.timeOfFade, TMPtext, nextWave, true));
    }

    public void ChangeToLoreText()
    {
        StopAllCoroutines();
        StartCoroutine(SwitchLoreTextDisplayed(0));
    }

}

[Serializable] public class LoreText
{
    public bool chooseThisTextAsFirstText;
    public string loreText;
    public float displayTime;
    public FadeHandeler fadeHandeler;
}
