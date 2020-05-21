using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FadeController : MonoBehaviour
{
    public CanvasGroupeHandeler[] groupeHandeler;
    public float timeBeforeSecondFade;
    public float timeBeforeThirdFade;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartEndGame();
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            StartEndGame();
    }


    public void StartEndGame()
    {
        StartCoroutine(StartsFades());
    }


    IEnumerator StartsFades()
    {
        StartCoroutine(LaunchFirstFade());
        yield return new WaitForSeconds(timeBeforeSecondFade);
        StartCoroutine(LaunchSecondFade());
        yield return new WaitForSeconds(timeBeforeThirdFade);
        StartCoroutine(LaunchThirdFade());
    }

    IEnumerator LaunchFirstFade()
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / groupeHandeler[0].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = groupeHandeler[0].fadeEffect.Evaluate(_currentTimeOfAnimation / groupeHandeler[0].timeOfFade);

            groupeHandeler[0].canvasGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        _currentTimeOfAnimation = 0;
    }

    IEnumerator LaunchSecondFade()
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / groupeHandeler[1].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = groupeHandeler[1].fadeEffect.Evaluate(_currentTimeOfAnimation / groupeHandeler[1].timeOfFade);

            groupeHandeler[1].canvasGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        _currentTimeOfAnimation = 0;
    }

    IEnumerator LaunchThirdFade()
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / groupeHandeler[2].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = groupeHandeler[2].fadeEffect.Evaluate(_currentTimeOfAnimation / groupeHandeler[2].timeOfFade);

            groupeHandeler[2].canvasGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
        groupeHandeler[0].canvasGroup.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[0].canvasGroup.GetComponent<CanvasGroup>().blocksRaycasts = true;
        groupeHandeler[2].canvasGroup.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[2].canvasGroup.GetComponent<CanvasGroup>().blocksRaycasts = true;
        _currentTimeOfAnimation = 0;
    }

}


[Serializable] public class CanvasGroupeHandeler
{
    public GameObject canvasGroup;
    public AnimationCurve fadeEffect;
    public float timeOfFade;
}
