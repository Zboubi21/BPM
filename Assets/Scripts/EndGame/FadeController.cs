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
    public float timeBeforeRollCredit;
    public UnityEvent eventOnStartOfEndGame;

    public EndGameScoreCount count;
    [Space]
    public Animator anim;
  

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
        PlayerController.s_instance.On_PlayerEnterInCinematicState(true);
        PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
        StartCoroutine(LaunchSecondFade());
        yield return new WaitForSeconds(timeBeforeThirdFade);
        yield return StartCoroutine(LaunchThirdFade());
        anim.SetTrigger("StartCredit");
        //StartCoroutine(AnimateText(animationController.fadeEffect, animationController.timeOfFade, animationController.objectToApplyFade, firstPos.position, finalPosition.position));
        //yield return new WaitForSeconds(timeBeforeRollCredit);
        //StartCoroutine(AnimateText(animationCreditController.fadeEffect, animationCreditController.timeOfFade, animationCreditController.objectToApplyFade, firstPosCredit.position, creditPosition.position));
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
        //count.StartCoroutine(count.StartCoutningScore());

        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / groupeHandeler[2].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = groupeHandeler[2].fadeEffect.Evaluate(_currentTimeOfAnimation / groupeHandeler[2].timeOfFade);

            groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, value);
            yield return null;

        }
        groupeHandeler[0].objectToApplyFade.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[0].objectToApplyFade.GetComponent<CanvasGroup>().blocksRaycasts = true;
        groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().interactable = true;
        groupeHandeler[2].objectToApplyFade.GetComponent<CanvasGroup>().blocksRaycasts = true;
        _currentTimeOfAnimation = 0;
    }

    IEnumerator AnimateText(AnimationCurve curve, float timeOfAnimation,GameObject objectToMove,Vector3 startPos, Vector3 endPos,  bool reverseFade = false)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation <= timeOfAnimation)
        {
            _currentTimeOfAnimation += Time.deltaTime;

            float value = curve.Evaluate(_currentTimeOfAnimation / timeOfAnimation);

            Vector3 posY = objectToMove.transform.position;
            posY = Vector3.Lerp(startPos, endPos, value);
            objectToMove.transform.position = posY;
            yield return null;
        }
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
