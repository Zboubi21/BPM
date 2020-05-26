using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class FadeController : MonoBehaviour
{
    public FadeOverCurveHandeler[] groupeHandeler;
    public float timeBeforeSecondFade;
    public float timeBeforeThirdFade;
    public UnityEvent eventOnStartOfEndGame;

    public EndGameScoreCount count;
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
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CurrentScore = 845120;
            StartEndGame();
        }
    }


    public void StartEndGame()
    {
        eventOnStartOfEndGame.Invoke();
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

            groupeHandeler[0].objectToApplyFade.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
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

            groupeHandeler[1].objectToApplyFade.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        _currentTimeOfAnimation = 0;
    }

    IEnumerator LaunchThirdFade()
    {
        count.StartCoroutine(count.StartCoutningScore());

        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / groupeHandeler[2].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = groupeHandeler[2].fadeEffect.Evaluate(_currentTimeOfAnimation / groupeHandeler[2].timeOfFade);

            groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
        groupeHandeler[0].objectToApplyFade.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[0].objectToApplyFade.GetComponent<CanvasGroup>().blocksRaycasts = true;
        groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().blocksRaycasts = true;
        _currentTimeOfAnimation = 0;
    }

}


[Serializable] public class FadeOverCurveHandeler
{
    public GameObject objectToApplyFade;
    public AnimationCurve fadeEffect;
    public float timeOfFade;
}[Serializable] public class FadeHandeler
{
    public AnimationCurve fadeEffect;
    public float timeOfFade;
}
