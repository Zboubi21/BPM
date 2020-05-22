using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFades : MonoBehaviour
{
    public FadeOverCurveHandeler FadeOverCurve;
    AudioSource audioSource;
    [Header("For fading in")]
    public float finalVolume;

    float previousVolume;
    public void StartFadeOut()
    {
        StartCoroutine(PlayFade(audioSource.volume, 0));
        previousVolume = audioSource.volume;
    }

    public void StartFadeIn()
    {
        if(previousVolume != 0)
        {
            StartCoroutine(PlayFade(0, previousVolume));
        }
        else
        {
            StartCoroutine(PlayFade(0, finalVolume));
        }
    }

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator PlayFade(float start, float end)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / FadeOverCurve.timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = FadeOverCurve.fadeEffect.Evaluate(_currentTimeOfAnimation / FadeOverCurve.timeOfFade);

            audioSource.volume = Mathf.Lerp(start, end, value);

            yield return null;
        }
        _currentTimeOfAnimation = 0;
    }
}
